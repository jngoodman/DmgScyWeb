using System.Data;
using Microsoft.Data.Sqlite;

namespace DmgScy;

public class PassDisplayData{
    BandService bandService;
    PageData indexBaseData;

    public PassDisplayData(){
        this.bandService = new BandService();
        this.indexBaseData = new PageData(Constants.Html.indexBase);
    }
    
    public void InitiateDatabase(){
        bandService.DatabaseCreate();
        bandService.DatabaseInsert();
    }

    public Dictionary<string, DataTable?>? GetBandsDatabase(){
        Dictionary<string, DataTable?>? returnValue = null;
        try{
            returnValue = bandService.DatabaseSelectWithHeader();
        }
        catch(SqliteException){
            Console.WriteLine("SqliteException. Database may not exist. Initialising...");
            try{
                InitiateDatabase();
                returnValue = bandService.DatabaseSelectWithHeader();
            }
            catch(SqliteException){
                Console.WriteLine("SqliteException. Returning null.");
            }
        }
        return returnValue;
    }

    public string? GetBandsHeader(){
        string? header = null;
        Dictionary<string, DataTable?>? dataTableWithHeader = GetBandsDatabase();
        if(dataTableWithHeader != null){
            header = dataTableWithHeader.First().Key;
        }
        return header;
    }

    public DataTable? GetBandsDataTable(){
        DataTable? dataTable = null;
        Dictionary<string, DataTable?>? dataTableWithHeader = GetBandsDatabase();
        if(dataTableWithHeader != null){
            dataTable = dataTableWithHeader.First().Value;
        }
        return dataTable;
    }

    public void WriteBandsToIndexHtml(){
        string? header = GetBandsHeader();
        DataTable? dataTable = GetBandsDataTable();        
        if((header != null) && (dataTable != null)){
            indexBaseData.WriteNewHTML(outfileLoc: Constants.Html.index, dataTable: dataTable, tableHeader: header);
        }
    }        
}