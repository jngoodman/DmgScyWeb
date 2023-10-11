namespace DmgScy;

public static class Run{
    public static void Main(string[] args){
        var pageData = new PageData(Constants.Html.index);
        var server = new Server(Constants.localhost, pageData.html);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}