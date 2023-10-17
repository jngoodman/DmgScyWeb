using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace DmgScy;

class Server{
    public HttpListener listener;
    public string url;
    public string indexData;
    public string pageData;

    public Server(string url, string indexData){
        this.url = url;
        this.indexData = indexData;
        this.pageData = indexData;
        this.listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    private void HandlePageData(HttpListenerRequest request){
        if (request.Url?.AbsoluteUri == url){
                pageData = indexData;
        }
        else{
            if(request.Url?.AbsolutePath == Constants.Html.shutdownCommand){     
                string fileLoc = Constants.Html.shutdown;
                HtmlReader shutdownReader = new HtmlReader(fileLoc); 
                pageData = shutdownReader.html;
            }
            else{     
                string fileLoc = Constants.Html.collectionBase;
                HtmlReader placeholderReader = new HtmlReader(fileLoc); 
                pageData = placeholderReader.html;
            }
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
            byte[] data = Encoding.UTF8.GetBytes(pageData);
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            }
            catch (HttpListenerException){
                Console.WriteLine("Connection temporarily dropped.");
            }
            response.Close();
        }
    }
}