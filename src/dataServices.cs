using System.Data;
using System.Data.SqlTypes;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;

namespace DmgScy;

public class BandService {
    public HandleDatabase databaseHandler;

    public BandService(string dataBase){
        this.databaseHandler = new HandleDatabase(dataBase); 
    }    

    public void DatabaseCreate(){
        string query = Constants.Sql.createBands;
        databaseHandler.RunQuery(query);
    }

    public DataTable DatabaseSelect(){
        string query = Constants.Sql.select.Replace("{tableName}", Constants.Sql.bandsTableName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Band> bandList){
        string query = Constants.Sql.addBands;
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@url", band.url)
            };
            databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }
}  

public class CollectionService{
    public HandleDatabase databaseHandler;
    public string tableName;

    public CollectionService(string tableName, string dataBase){
        this.databaseHandler = new HandleDatabase(dataBase);
        this.tableName = tableName;
    }    

    public void DatabaseCreate(){
        string query = Constants.Sql.createCollection.Replace("{tableName}", tableName);
        databaseHandler.RunQuery(query);
    }

    public DataTable DatabaseSelect(){
        string query = Constants.Sql.select.Replace("{tableName}", tableName);
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
        string query = Constants.Sql.addCollection.Replace("{tableName}", tableName);
        databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }      
}

public class FavouritesHandler{
    public HandleDatabase databaseHandler;

    public FavouritesHandler(){
        this.databaseHandler = new HandleDatabase(dataSource: Constants.Sql.dataSource);
    }
    

    public void CreateFavourites(){
        string query = Constants.Sql.createFavourites;
        databaseHandler.RunQuery(query);
    }

    public void AddFavourite(string bandName){
        string query = Constants.Sql.replaceState.Replace("{name}", bandName);
        query = query.Replace("{newState}", Constants.favIcon);
        databaseHandler.RunQuery(query);   
    }

    public void RemoveFavourite(string bandName){
        string query = Constants.Sql.replaceState.Replace("{name}", bandName);
        query = query.Replace("{newState}", Constants.notFavIcon);
        databaseHandler.RunQuery(query);   
    }

    public DataTable SelectState(string bandName){
        string query = Constants.Sql.selectState.Replace("{name}", bandName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }
    
    public void InsertStates(List<Band> bandList){
        string query = Constants.Sql.insertFavourites;
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@state", Constants.notFavIcon)
            };            
        databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }
}

public class DataServiceManager{
    public BandOrCollectionService dataService;
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
