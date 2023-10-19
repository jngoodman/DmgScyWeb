using System.Data;
using OneOf;

namespace DmgScy;

public static class Run{
    
    public static void Main(){        
        if(DataCleaner.ShouldRefresh()){
            DataCleaner.ClearTempData();
        }
        Server server = new Server(Constants.localhost);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}