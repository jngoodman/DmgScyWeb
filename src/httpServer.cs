using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Storage;

namespace DmgScy;

class Server{
    public HttpListener listener;
    public string url;
    public string? currPage;

    public Server(string url){
        this.url = url;
        this.currPage = null;
        this.listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    private void ServeIndex(){
        PageBuilder pageBuilder = new PageBuilder(PageType.INDEX, pageName: Constants.Sql.bandsTableName);
        pageBuilder.Build(sourceUrl: Constants.all_bands_url, fromLocal: TableExists.Check(Constants.Sql.bandsTableName, Constants.Sql.dataSource));
        HtmlReader pageReader = new HtmlReader(Constants.Html.index);
        currPage = pageReader.html;
    }

    private void ServeCollection(string bandName){
        PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: bandName);
        string sourceUrl = PullBandUrl(bandName);
        pageBuilder.Build(sourceUrl: sourceUrl, fromLocal: TableExists.Check(StringCleaner.EraseIllegalChars(bandName), dataBase: Constants.Sql.dataSource));
        HtmlReader pageReader = new HtmlReader(Constants.Html.collectionLast);
        currPage = pageReader.html;
    }

    private void ServeShutdown(){
        HtmlReader shutdownReader = new HtmlReader(Constants.Html.shutdown); 
        currPage = shutdownReader.html;
    }

    private void ToggleFavourites(string bandName, HttpListenerResponse response){
        BandService bandService = new BandService(dataBase: Constants.Sql.dataSource, tableName: Constants.Sql.bandsTableName);
        string currentState = $"{bandService.PullState(bandName).Rows[0][0]}";            
        if(currentState == Constants.favIcon){
            bandService.RemoveFavourite(bandName);
        }
        else if(currentState == Constants.notFavIcon){
            bandService.AddFavourite(bandName);
        }
        response.Redirect(Constants.indexUrls[0]);
    }

    private void ServeFavourites(){   
        BandService bandService = new BandService(dataBase: Constants.Sql.dataSource, tableName: Constants.Sql.bandsTableName);
        CollectionService favouritesService = new CollectionService(dataBase: Constants.Sql.dataSource, tableName: Constants.Sql.favTableName);
        favouritesService.DatabaseCreate();
        foreach(DataRow row in bandService.SelectFavourited().Rows){
            string bandName = $"{row["name"]}";
            string bandUrl = $"{row["url"]}";
            CollectionService collectionService = new CollectionService(tableName: StringCleaner.EraseIllegalChars(bandName), dataBase: Constants.Sql.dataSource); 
            if(!TableExists.Check(StringCleaner.EraseIllegalChars(bandName), Constants.Sql.dataSource)){
                PageData collectionData = new PageData(collectionService);
                BandOrCollectionList? dataList = collectionData.ScrapeWebData(url: bandUrl, cssSelector: Constants.collection_css_selector);
                if(dataList != null){
                    favouritesService.DatabaseInsert(dataList.AsT1);  
                }             
            }
            else{
                DataTable dataTable = collectionService.DatabaseSelect();
                List<Collection> dataList = new List<Collection>();
                foreach(DataRow itemRow in dataTable.Rows){
                    string name  = $"{itemRow["name"]}";
                    string url  = $"{itemRow["url"]}";
                    string price  = $"{itemRow["price"]}";
                    string image  = $"{itemRow["image"]}";
                    Collection collection = new Collection(name: name, url: url, price: price, image: image);
                    dataList.Add(collection);
                }
                favouritesService.DatabaseInsert(dataList);
            }
        }
        PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: Constants.Sql.favTableName);
        pageBuilder.Build(sourceUrl: "", fromLocal: true);
        HtmlReader pageReader = new HtmlReader(Constants.Html.collectionLast);
        currPage = pageReader.html;
    }

    private string PullBandUrl(string bandName){
        BandService bandService = new BandService(dataBase: Constants.Sql.dataSource, tableName: Constants.Sql.bandsTableName);
        string url = $"{bandService.PullUrl(bandName).Rows[0][0]}";
        return url;
        }

    private void HandlePageData(HttpListenerRequest request, HttpListenerResponse response){
        string? url = request.Url?.AbsoluteUri;
        if(url != null){
            if(Constants.indexUrls.Contains(url)){
                ServeIndex();
            }
            else if(url.Contains(Constants.Html.shutdownCommand)){
                ServeShutdown();
            }
            else if(url.Contains(Constants.Html.favouritesRightPart)){
                ServeFavourites();
            }
            else{
                string? rightPart = request.Url?.AbsolutePath;
                if(rightPart != null){
                    string bandName = HttpUtility.UrlDecode(rightPart).TrimStart('/');
                    if(url.Contains(Constants.Html.favPart)){
                        bandName = bandName.Split(Constants.Html.favPart)[1];
                        ToggleFavourites(bandName, response);
                    }
                    else{
                        ServeCollection(bandName);
                    }
                }
            }
        }
    }

    public async Task HandleIncomingConnections(){
        bool runServer = true;
        while(runServer){    
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            HttpListenerRequest request = listenerContext.Request;
            HttpListenerResponse response = listenerContext.Response;
            HandlePageData(request, response);
            if ((request.HttpMethod == "POST") && (request.Url?.AbsolutePath == Constants.Html.shutdownCommand)){
                Console.WriteLine("Shutdown requested.");
                runServer = false;
            }
            try {
                if(currPage != null){
                    byte[] data = Encoding.UTF8.GetBytes(currPage);
                    response.ContentType = "text/html";
                    response.ContentLength64 = data.LongLength;
                    await response.OutputStream.WriteAsync(data, 0, data.Length);
                }
                else{
                    Console.WriteLine("Error! Page data missing.");
                };
            }
            catch (HttpListenerException){
                Console.WriteLine("Connection temporarily dropped.");
            }
            response.Close();
        }
    }
}