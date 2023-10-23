using System.Data.Common;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using HtmlAgilityPack;
using System.Reflection.Metadata;

namespace DmgScy;


public class HandleDatabase{
    string connectionString;
    SqliteConnection dbConnection;

    public HandleDatabase(string dataSource){
        this.connectionString = $"Data Source={dataSource}";
        this.dbConnection = new SqliteConnection(connectionString);
    }
        
    private DataTable ExecuteCommand(SqliteCommand command, bool returnTable = false){
        DataTable returnValue = new DataTable();
        if(!returnTable){
            command.ExecuteNonQuery();
            return returnValue;
        }       
        SqliteDataReader reader = command.ExecuteReader();
        for(int columnNumber = 0; columnNumber < reader.FieldCount; columnNumber++){
            DataColumn column = new DataColumn(reader.GetName(columnNumber));
            returnValue.Columns.Add(column);
            }
        int rowNumber = 0;
        while(reader.Read()){
            DataRow row = returnValue.NewRow();
            returnValue.Rows.Add(row);
            for(int columnNumber = 0; columnNumber < reader.FieldCount; columnNumber++){
                returnValue.Rows[rowNumber][columnNumber] = reader.GetValue(columnNumber);
            }
            rowNumber++;
        }
        return returnValue;
    }

    public DataTable RunQuery(string commandString, List<SqliteParameter>? parameters = null, bool returnTable = false){
        SqliteCommand command = new SqliteCommand(commandString, dbConnection);
        if(parameters != null){
            foreach(SqliteParameter parameter in parameters){
                command.Parameters.Add(parameter);
            }
        }
        dbConnection.Open();
        DataTable? dataTable = ExecuteCommand(command, returnTable);
        dbConnection.Close();
        return dataTable;
    }
}

public static class TableExists{

    public static bool Check(string tableName, string dataBase){
        HandleDatabase databaseHandler = new HandleDatabase(dataBase);
        try {
            string query = Constants.Sql.select.Replace("{name}", tableName);
            databaseHandler.RunQuery(query);
            return true;
        }
        catch(SqliteException){
            return false;
        }
    }
}

public static class DataCleaner{

    public static bool ShouldRefresh(){
        if(File.Exists(Constants.Sql.refreshToken)){
            DateTime creationTime = File.GetCreationTime(Constants.Sql.refreshToken);
            DateTime currentTime = DateTime.Now;
            TimeSpan span = currentTime.Subtract(creationTime);
            Console.WriteLine($"Refresh token is {span.Hours} hours old. Existing data will be overwritten after {Constants.refreshHours} hours.");
            if(span.Hours >= Constants.refreshHours){
                Console.WriteLine("Refreshing database... updated merchandise data is being requested...");
                File.Create(Constants.Sql.refreshToken);
                return true;
            }
            Console.WriteLine("Database not refreshed. Where possible, locally-stored data will be used.");
            return false;
        }
        Console.WriteLine("Refresh token missing. Creating token now.");
        File.Create(Constants.Sql.refreshToken);
        return false;
    }

    public static void ClearTempData(){
        BandService bandService = new BandService(tableName: Constants.Sql.bandsTableName, dataBase: Constants.Sql.dataSource);
        HandleDatabase databaseHandler = new HandleDatabase(dataSource: Constants.Sql.dataSource);
        PageData tempData = new PageData(new BandService(tableName: "temp", dataBase: Constants.Sql.dataSource));
        tempData.ScrapeWebData(url: Constants.all_bands_url, cssSelector: Constants.band_css_selector);
        databaseHandler.RunQuery(commandString: Constants.Sql.dropTable.Replace("{tableName}", Constants.Sql.favTableName));
        foreach(DataRow row in bandService.DatabaseSelect().Rows){
            string bandName = $"{row["name"]}";
            string state = $"{row["state"]}";
            databaseHandler.RunQuery(commandString: Constants.Sql.dropTable.Replace("{tableName}", StringCleaner.EraseIllegalChars(bandName)));
            if(state == Constants.favIcon){
                tempData.dataService.AsT0.AddFavourite(bandName);
            }
        }
        databaseHandler.RunQuery(commandString: Constants.Sql.dropTable.Replace("{tableName}", Constants.Sql.bandsTableName));
        string renameQuery = Constants.Sql.renameTable.Replace("{tableName}", "temp");
        renameQuery = renameQuery.Replace("{newName}", Constants.Sql.bandsTableName);
        databaseHandler.RunQuery(commandString: renameQuery);
    }
}