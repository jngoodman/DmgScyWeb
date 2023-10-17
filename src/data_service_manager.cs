using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DmgScy;
public class DataServiceManager{
    public BandOrCollectionService  dataService;
    public DataTarget dataTarget;

    public DataServiceManager(BandOrCollectionService dataService){
        this.dataService = dataService;
    }
    
    public void CreateTable(){        
        if(dataService.IsT0){
            dataService.AsT0.DatabaseCreate();
        }
        else if(dataService.IsT1){
            dataService.AsT1.DatabaseCreate();
        }
    }

    public void FillTable(BandOrCollectionList insertList){        
        try{
            if(dataService.IsT0){
                dataService.AsT0.DatabaseInsert(insertList.AsT0);
            }
            else if(dataService.IsT1){
                dataService.AsT1.DatabaseInsert(insertList.AsT1);
            }
        }
        catch(SqliteException){
            Console.WriteLine("Sqlite error. Check insertList matches dimensions of table.");
        }
    }

    public DataTable? SelectTable(){
        DataTable? returnValue = null;
        try{
            if(dataService.IsT0){
                returnValue = dataService.AsT0.DatabaseSelect();
            }
            else if(dataService.IsT1){
                returnValue = dataService.AsT1.DatabaseSelect();
            }
        }
        catch(SqliteException){
            Console.WriteLine("Sqlite error. Check table exists. Returning null.");
        }
        return returnValue;
    }

    public DataTable? GetDataTable(BandOrCollectionList insertList){
        CreateTable();
        FillTable(insertList);
        return SelectTable();
    } 
}

