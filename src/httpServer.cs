using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Data;

namespace DmgScy;

class Server{
    public HttpListener listener;
    public string url;
    public string? currPage;
    private List<Band> bands;

    public Server(string url){
        this.url = url;
        this.currPage = null;
        this.bands = new List<Band>();
        this.listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    private void ServeIndex(){
        PageBuilder pageBuilder = new PageBuilder(PageType.INDEX, pageName: "bands");
        pageBuilder.Build(sourceUrl: Constants.all_bands_url, TableExists.Check("bands", Constants.Sql.dataSource));
        DataTable? bandTable = pageBuilder.pageData.dataTable;
        if((bandTable != null)&&(!bands.Any())){
            foreach(DataRow row in bandTable.Rows){
                string bandName = $"{row["name"]}";
                string bandUrl = $"{row["url"]}";
                bands.Add(new Band(name: bandName, url: bandUrl));
            }
        }
        HtmlReader pageReader = new HtmlReader(Constants.Html.index);
        currPage = pageReader.html;
    }

    private void ServeCollection(string sourceUrl, string sourceName){
        if(sourceUrl != null){
            string pageName = StringCleaner.EraseIllegalChars(sourceName);
            PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: pageName);
            pageBuilder.Build(sourceUrl: sourceUrl, TableExists.Check(pageName, Constants.Sql.dataSource));
            HtmlReader pageReader = new HtmlReader(Constants.Html.collectionLast);
            currPage = pageReader.html;
        }
    }

    private void ServeShutdown(){
        HtmlReader shutdownReader = new HtmlReader(Constants.Html.shutdown); 
        currPage = shutdownReader.html;
    }

    private void AddToFavourites(string sourceUrl, string sourceName, HttpListenerResponse response){
        if(sourceUrl != null){
            BandService favouritesService = new BandService(dataBase: Constants.Sql.dataSource, tableName: "favourites");
            favouritesService.DatabaseCreate();
            Band band = new Band(name: sourceName, url: sourceUrl);
            favouritesService.DatabaseInsertSingle(band);
        }
        response.Redirect(Constants.localhost);
    }

    private string PullSourceUrl(string sourceName){
        string sourceUrl = "";
        if(bands != null){
            Band? band = bands.Find(x => x.name == sourceName);
            if(band != null){
                sourceUrl = band.url;
                }
            }
        return sourceUrl;        
    }

    private string PullSourceName(HttpListenerRequest request){
        string sourceName = "";
        string? partialUrl = request.Url?.AbsolutePath;
        if(partialUrl != null){
            string bandName = HttpUtility.UrlDecode(partialUrl);
            sourceName = bandName.TrimStart('/');
        }
        return sourceName;        
    }

    private void HandlePageData(HttpListenerRequest request, HttpListenerResponse response){
        if((request.Url?.AbsoluteUri == url) || (request.Url?.AbsolutePath == "favicon.ico")){
            ServeIndex();
        }
        else if(request.Url?.AbsolutePath == Constants.Html.shutdownCommand){
            ServeShutdown();
        }
        else{
            string sourceName = PullSourceName(request);
            if(sourceName.Contains(Constants.Html.favMarkerUrl)){
                sourceName = sourceName.Replace(Constants.Html.favMarkerUrl, "");
                string sourceUrl = PullSourceUrl(sourceName);
                AddToFavourites(sourceUrl, sourceName, response);
            }
            else{
                string sourceUrl = PullSourceUrl(sourceName);
                Console.WriteLine(request.Url?.AbsoluteUri);
                Console.WriteLine(sourceUrl);
                ServeCollection(sourceUrl, sourceName);
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