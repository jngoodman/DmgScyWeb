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
        string query = Constants.Sql.selectBands.Replace("{indexName}", tableName);
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
        string query = Constants.Sql.selectCollection.Replace("{collectionName}", tableName);
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