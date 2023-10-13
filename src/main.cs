using System.Data;

namespace DmgScy;

public static class Run{
    public static void Main(string[] args){
        BandService bandService = new BandService();
        List<Band> bandList = bandService.GetBands();
        HandleDatabase databaseHandler = new HandleDatabase();
        databaseHandler.RunQuery(Constants.Sql.createBands);
        List<string> parameters = new List<string>();
        foreach(Band band in bandList){
            parameters.Add(band.name);
            parameters.Add(band.url);
        }
        databaseHandler.RunQuery(Constants.Sql.addBands, parameters: parameters);
        DataTable? dataTable = databaseHandler.RunQuery(Constants.Sql.selectBands, returnTable: true);
        var baseData = new PageData(Constants.Html.indexBase);
        if(dataTable != null){
            baseData.WriteNewHTML(outfileLoc: Constants.Html.index, dataTable: dataTable);
        }
        var pageData = new PageData(Constants.Html.index);
        var server = new Server(Constants.localhost, pageData.html);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}