using System.Net;
using System.Text;

namespace DmgScy;

class Server{
    public HttpListener listener;
    public string url;
    public string pageData;

    public Server(string url, string pageData){
        this.url = url;
        this.pageData = pageData;
        this.listener = new HttpListener();
        listener.Prefixes.Add(url);
    }

    public async Task HandleIncomingConnections(){
        bool runServer = true;
        while(runServer){    
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            HttpListenerRequest request = listenerContext.Request;
            HttpListenerResponse response = listenerContext.Response;
            if ((request.HttpMethod == "POST") && (request.Url?.AbsolutePath == "/shutdown")){
                    Console.WriteLine("Shutdown requested.");
                    runServer = false;
            }
            byte[] data = Encoding.UTF8.GetBytes(pageData);
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
        }
    }
}