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
        PageBuilder pageBuilder = new PageBuilder(PageType.INDEX, pageName: Constants.InternalStorage.Tables.bands);
        pageBuilder.Build(sourceUrl: Constants.ExternalUrls.allBandsUrl, fromLocal: TableExists.Check(Constants.InternalStorage.Tables.bands, Constants.InternalStorage.dataBase));
        HtmlReader pageReader = new HtmlReader(Constants.Html.Templates.index);
        currPage = pageReader.html;
    }

    private void ServeCollection(string bandName){
        PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: bandName);
        string sourceUrl = PullBandUrl(bandName);
        pageBuilder.Build(sourceUrl: sourceUrl, fromLocal: TableExists.Check(StringCleaner.EraseIllegalChars(bandName), dataBase: Constants.InternalStorage.dataBase));
        HtmlReader pageReader = new HtmlReader(Constants.Html.Templates.collection);
        currPage = pageReader.html;
    }

    private void ServeShutdown(){
        HtmlReader shutdownReader = new HtmlReader(Constants.Html.Templates.shutdown); 
        currPage = shutdownReader.html;
    }

    private void ToggleFavourites(string bandName, HttpListenerResponse response){
        BandService bandService = new BandService(dataBase: Constants.InternalStorage.dataBase, tableName: Constants.InternalStorage.Tables.bands);
        string currentState = $"{bandService.PullState(bandName).Rows[0][0]}";            
        if(currentState == Constants.InternalStorage.Images.favourited){
            bandService.RemoveFavourite(bandName);
        }
        else if(currentState == Constants.InternalStorage.Images.notFavourited){
            bandService.AddFavourite(bandName);
        }
        response.Redirect(Constants.InternalUrls.indexUrls[0]);
    }

    private void ServeFavourites(){   
        BandService bandService = new BandService(dataBase: Constants.InternalStorage.dataBase, tableName: Constants.InternalStorage.Tables.bands);
        CollectionService favouritesService = new CollectionService(dataBase: Constants.InternalStorage.dataBase, tableName: Constants.InternalStorage.Tables.favourites);
        favouritesService.DatabaseCreate();
        favouritesService.InsertFavouritedCollections(bandService);
        PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: Constants.InternalStorage.Tables.favourites);
        pageBuilder.Build(sourceUrl: "", fromLocal: true);
        HtmlReader pageReader = new HtmlReader(Constants.Html.Templates.collection);
        currPage = pageReader.html;
    }

    private string PullBandUrl(string bandName){
        BandService bandService = new BandService(dataBase: Constants.InternalStorage.dataBase, tableName: Constants.InternalStorage.Tables.bands);
        string url = $"{bandService.PullUrl(bandName).Rows[0][0]}";
        return url;
        }

    private void HandlePageData(HttpListenerRequest request, HttpListenerResponse response){
        string? url = request.Url?.AbsoluteUri;
        if(url != null){
            if(Constants.InternalUrls.indexUrls.Contains(url)){
                ServeIndex();
            }
            else if(url.Contains(Constants.InternalUrls.shutdownCommand)){
                ServeShutdown();
            }
            else if(url.Contains(Constants.InternalUrls.favouritesCommand)){
                ServeFavourites();
            }
            else{
                string? rightPart = request.Url?.AbsolutePath;
                if(rightPart != null){
                    string bandName = HttpUtility.UrlDecode(rightPart).TrimStart('/');
                    if(url.Contains(Constants.InternalUrls.favouritesSplitter)){
                        bandName = bandName.Split(Constants.InternalUrls.favouritesSplitter)[1];
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
            if ((request.HttpMethod == "POST") && (request.Url?.AbsolutePath == Constants.InternalUrls.shutdownCommand)){
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
                Console.WriteLine("Temporary connection issue.");
            }
            response.Close();
        }
    }
}