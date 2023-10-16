using System.Data;

namespace DmgScy;

public static class Run{
    
    public static void Main(string[] args){
        PassDisplayData displayData = new PassDisplayData();
        displayData.InitiateDatabase();
        displayData.WriteBandsToIndexHtml();
        PageData indexData = new PageData(Constants.Html.index);
        Server server = new Server(Constants.localhost, indexData.html);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}