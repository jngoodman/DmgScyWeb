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
            string query = Constants.InternalStorage.SqlCommands.select.Replace("{name}", tableName);
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
        if(File.Exists(Constants.InternalStorage.refreshToken)){
            DateTime creationTime = File.GetCreationTime(Constants.InternalStorage.refreshToken);
            DateTime currentTime = DateTime.Now;
            TimeSpan span = currentTime.Subtract(creationTime);
            Console.WriteLine($"Refresh token is {span.Hours} hours old. Existing data will be overwritten after {Constants.InternalStorage.refreshHours} hours.");
            if(span.Hours >= Constants.InternalStorage.refreshHours){
                Console.WriteLine("Refreshing database... updated merchandise data is being requested...");
                File.Create(Constants.InternalStorage.refreshToken);
                return true;
            }
            Console.WriteLine("Database not refreshed. Where possible, locally-stored data will be used.");
            return false;
        }
        Console.WriteLine("Refresh token missing. Creating token now.");
        File.Create(Constants.InternalStorage.refreshToken);
        return false;
    }

    public static void ClearTempData(){
        BandService bandService = new BandService(tableName: Constants.InternalStorage.Tables.bands, dataBase: Constants.InternalStorage.dataBase);
        HandleDatabase databaseHandler = new HandleDatabase(dataSource: Constants.InternalStorage.dataBase);
        PageData tempData = new PageData(new BandService(tableName: "temp", dataBase: Constants.InternalStorage.dataBase));
        tempData.ScrapeWebData(url: Constants.ExternalUrls.allBandsUrl, cssSelector: Constants.Selectors.bandCssSelector);
        databaseHandler.RunQuery(commandString: Constants.InternalStorage.SqlCommands.dropTable.Replace("{tableName}", Constants.InternalStorage.Tables.favourites));
        foreach(DataRow row in bandService.DatabaseSelect().Rows){
            string bandName = $"{row["name"]}";
            string state = $"{row["state"]}";
            databaseHandler.RunQuery(commandString: Constants.InternalStorage.SqlCommands.dropTable.Replace("{tableName}", StringCleaner.EraseIllegalChars(bandName)));
            if(state == Constants.InternalStorage.Images.favourited){
                tempData.dataService.AsT0.AddFavourite(bandName);
            }
        }
        databaseHandler.RunQuery(commandString: Constants.InternalStorage.SqlCommands.dropTable.Replace("{tableName}", Constants.InternalStorage.Tables.bands));
        string renameQuery = Constants.InternalStorage.SqlCommands.renameTable.Replace("{tableName}", "temp");
        renameQuery = renameQuery.Replace("{newName}", Constants.InternalStorage.Tables.bands);
        databaseHandler.RunQuery(commandString: renameQuery);
    }
}