using System.Data;
using System.Data.SqlTypes;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;

namespace DmgScy;

public class BandService {
    public HandleDatabase databaseHandler;
    public string tableName;

    public BandService(string dataBase, string tableName){
        this.databaseHandler = new HandleDatabase(dataBase); 
        this.tableName = tableName;
    }    

    public void DatabaseCreate(){
        string query = Constants.Sql.createBands.Replace("{tableName}", tableName);
        databaseHandler.RunQuery(query);
    }

    public DataTable DatabaseSelect(){
        string query = Constants.Sql.select.Replace("{tableName}", tableName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Band> bandList){
        string query = Constants.Sql.addBands.Replace("{tableName}", tableName);
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@url", band.url),
                new SqliteParameter("@state", band.state)
            };
            databaseHandler.RunQuery(query, parameters: parameterList);
        }
    }

     public void AddFavourite(string bandName){
        string query = Constants.Sql.updateValue.Replace("{condition}", bandName);
        query = query.Replace("{newValue}", Constants.favIcon);
        query = query.Replace("{targetColumn}", "state");
        query = query.Replace("{conditionColumn}", "name");
        query = query.Replace("{tableName}", tableName);
        databaseHandler.RunQuery(query);
    }

    public void RemoveFavourite(string bandName){
        string query = Constants.Sql.updateValue.Replace("{condition}", bandName);
        query = query.Replace("{newValue}", Constants.notFavIcon);
        query = query.Replace("{targetColumn}", "state");
        query = query.Replace("{conditionColumn}", "name");
        query = query.Replace("{tableName}", tableName);
        databaseHandler.RunQuery(query);   
    }    
    
    public DataTable SelectFavourited(){
        string query = Constants.Sql.selectFrom.Replace("{tableName}", tableName);
        query = query.Replace("{targetColumn}", "name, url");
        query = query.Replace("{conditionColumn}", "state");
        query = query.Replace("{condition}", Constants.favIcon);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }
    
    public DataTable PullState(string bandName){
        string query = Constants.Sql.selectFrom.Replace("{tableName}", tableName);
        query = query.Replace("{targetColumn}", "state");
        query = query.Replace("{conditionColumn}", "name");
        query = query.Replace("{condition}", bandName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
    }
    
    public DataTable PullUrl(string bandName){
        string query = Constants.Sql.selectFrom.Replace("{tableName}", tableName);
        query = query.Replace("{targetColumn}", "url");
        query = query.Replace("{conditionColumn}", "name");
        query = query.Replace("{condition}", bandName);
        DataTable dataTable = databaseHandler.RunQuery(query, returnTable: true);
        return dataTable;
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