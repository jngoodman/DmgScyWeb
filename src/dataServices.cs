using System.Data;
using System.Data.SqlTypes;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;

namespace DmgScy;

public interface IDataService{
    public HandleDatabase databaseHandler {get; set; }
    public string tableName {get; set; }
}

public class BandService: IDataService {
    public HandleDatabase databaseHandler {get; set; }
    public string tableName {get; set; }

    public BandService(string tableName){
        this.databaseHandler = new HandleDatabase(); 
        this.tableName = tableName;            
    }    

    public void DatabaseCreate(){
        string query = Constants.Sql.createBands.Replace("{indexName}", tableName);
        databaseHandler.RunQuery(query);
    }

    public DataTable DatabaseSelect(){
        string query = Constants.Sql.select.Replace("{name}", tableName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Band> bandList){
        string query = Constants.Sql.addBands.Replace("{indexName}", tableName);
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@url", band.url)
            };
            databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }    
}

public class CollectionService: IDataService {
    public HandleDatabase databaseHandler {get; set; }
    public string tableName {get; set; }

    public CollectionService(string tableName){
        this.databaseHandler = new HandleDatabase();
        this.tableName = tableName;
    }    

    public void DatabaseCreate(){
        string query = Constants.Sql.createCollection.Replace("{collectionName}", tableName);
        databaseHandler.RunQuery(query);
    }

    public DataTable DatabaseSelect(){
        string query = Constants.Sql.select.Replace("{name}", tableName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Collection> itemList){
        foreach(Collection item in itemList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", item.name),
                new SqliteParameter("@url", item.url),
                new SqliteParameter("@price", item.price),
                new SqliteParameter("@image", item.image)
            };
        string query = Constants.Sql.addCollection.Replace("{collectionName}", tableName);
        databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }      
}

public class DataServiceManager{
    public BandOrCollectionService  dataService;
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

    public DataTable SelectTable(){
        DataTable dataTable = new DataTable();
        try{
            if(dataService.IsT0){
                dataTable = dataService.AsT0.DatabaseSelect();
            }
            else if(dataService.IsT1){
                dataTable = dataService.AsT1.DatabaseSelect();
            }
        }
        catch(SqliteException){
            Console.WriteLine("Sqlite error. Check table exists. Returning null.");
        }
        return dataTable;
    }

    public void GenDataTable(BandOrCollectionList insertList){
        CreateTable();
        FillTable(insertList);
    } 
}
