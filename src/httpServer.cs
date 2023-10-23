using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace DmgScy;

class Server{
    public HttpListener listener;
    public string url;
    public string? currPage;
    private List<Band> bands;
    private FavouritesHandler favouritesHandler;

    public Server(string url){
        this.url = url;
        this.currPage = null;
        this.bands = new List<Band>();
        this.listener = new HttpListener();
        this.favouritesHandler = new FavouritesHandler();
        listener.Prefixes.Add(url);
    }

    private void ServeIndex(){
        PageBuilder pageBuilder = new PageBuilder(PageType.INDEX, pageName: Constants.Sql.bandsTableName);
        pageBuilder.Build(sourceUrl: Constants.all_bands_url, fromLocal: TableExists.Check(Constants.Sql.bandsTableName, Constants.Sql.dataSource));
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

    private void ServeCollection(string bandName){
        PageBuilder pageBuilder = new PageBuilder(PageType.COLLECTION, pageName: bandName);
        string? sourceUrl = PullBandUrl(bandName, bands);
        if(sourceUrl != null){
            pageBuilder.Build(sourceUrl: sourceUrl, TableExists.Check(StringCleaner.EraseIllegalChars(bandName), Constants.Sql.dataSource));
            HtmlReader pageReader = new HtmlReader(Constants.Html.collectionLast);
            currPage = pageReader.html;
        }
    }

    private void ServeShutdown(){
        HtmlReader shutdownReader = new HtmlReader(Constants.Html.shutdown); 
        currPage = shutdownReader.html;
    }

    private void ToggleFavourites(string bandName, HttpListenerResponse response){   
        string? currentState = favouritesHandler.SelectState(bandName).Rows[0][0].ToString();
        if(currentState == Constants.favIcon){
            favouritesHandler.RemoveFavourite(bandName);
        }
        else if(currentState == Constants.notFavIcon){
            favouritesHandler.AddFavourite(bandName);
        }
        response.Redirect(Constants.indexUrls[0]);
    }

    private string? PullBandUrl(string bandName, List<Band> bands){
        Band? band = bands.Find(x => x.name == bandName);
        if(band != null){
            return band.url;
            }
        return null;
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
            else{
                string? rightPart = request.Url?.AbsolutePath;
                if(rightPart != null){
                    string bandName = HttpUtility.UrlDecode(rightPart).TrimStart('/');
                    if(url.Contains(Constants.Html.favMarkerUrl)){
                        bandName = bandName.Split(Constants.Html.favMarkerUrl)[1];
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