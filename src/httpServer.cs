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
        if(bandTable != null){
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
        if(sourceUrl != ""){
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

    private void AddToFavourites(){
        
        ServeIndex();
    }

    private string PullSourceUrl(HttpListenerRequest request){
        string sourceUrl = "";
        string? partialUrl = request.Url?.AbsolutePath;
        if(partialUrl != null){
            string bandName = HttpUtility.UrlDecode(partialUrl);
            bandName = bandName.TrimStart('/');
            if(bands != null){
                Band? band = bands.Find(x => x.name == bandName);
                if(band != null){
                    sourceUrl = band.url;
                }
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

    private void HandlePageData(HttpListenerRequest request){
        if(request.Url?.AbsoluteUri == url){
            ServeIndex();
        }
        else if(request.Url?.AbsolutePath == Constants.Html.shutdownCommand){
            ServeShutdown();
        }
        else{
            string sourceUrl = PullSourceUrl(request); 
            string sourceName = PullSourceName(request);
            ServeCollection(sourceUrl, sourceName);    
        }
    }

    public async Task HandleIncomingConnections(){
        bool runServer = true;
        while(runServer){    
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            HttpListenerRequest request = listenerContext.Request;
            HttpListenerResponse response = listenerContext.Response;
            HandlePageData(request);
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