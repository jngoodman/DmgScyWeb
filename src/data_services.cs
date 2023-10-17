using System.Data;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;

namespace DmgScy;

public enum DataTarget{
    BAND,
    COLLECTION
}

public interface IDataService{
    public HandleDatabase databaseHandler {get; set; }
}

public class BandService: IDataService {
    public HandleDatabase databaseHandler {get; set; }

    public BandService(){
        this.databaseHandler = new HandleDatabase();        
    }    

    public void DatabaseCreate(){
        databaseHandler.RunQuery(Constants.Sql.createBands);
    }

    public DataTable? DatabaseSelect(){
        DataTable? dataTable = databaseHandler.RunQuery(Constants.Sql.selectBands, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Band> bandList){
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@url", band.url)
            };
            databaseHandler.RunQuery(Constants.Sql.addBands, parameters: parameterList);
        }
    }    
}

public class CollectionService: IDataService {
    public HandleDatabase databaseHandler {get; set; }

    public CollectionService(string url){
        this.databaseHandler = new HandleDatabase();
    }    

    public void DatabaseCreate(){
        databaseHandler.RunQuery(Constants.Sql.createBands);
    }

    public DataTable? DatabaseSelect(){
        DataTable? dataTable = databaseHandler.RunQuery(Constants.Sql.selectBands, returnTable: true);
        return dataTable;
    }

    public void DatabaseInsert(List<Collection> itemList){
        foreach(Collection item in itemList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", item.name),
                new SqliteParameter("@url", item.url)
            };
            databaseHandler.RunQuery(Constants.Sql.addBands, parameters: parameterList);
        }
    }      
}