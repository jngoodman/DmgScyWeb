using System.Data;

namespace DmgScy;

public static class Run{
    public static void Main(string[] args){
        BandService bandService = new BandService();
        bandService.DatabaseCreate();
        bandService.DatabaseInsert();
        Dictionary<string, DataTable?> dataTableWithHeader = bandService.DatabaseSelectWithHeader();
        string header = dataTableWithHeader.First().Key;
        DataTable? dataTable = dataTableWithHeader.First().Value;
        var baseData = new PageData(Constants.Html.indexBase);
        if(dataTable!= null){
            baseData.WriteNewHTML(outfileLoc: Constants.Html.index, dataTable: dataTable, tableHeader: header);
        }
        var pageData = new PageData(Constants.Html.index);
        var server = new Server(Constants.localhost, pageData.html);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}