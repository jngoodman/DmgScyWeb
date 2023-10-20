using System.Data.Common;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        DateTime creationTime = File.GetCreationTime(Constants.Sql.dataSource);
        DateTime currentTime = DateTime.Now;
        TimeSpan span = currentTime.Subtract(creationTime);
        if(span.Days >= Constants.refreshDays){
            return true;
        }
        return false;
    }

    public static void ClearTempData(){
        File.Delete(Constants.Sql.dataSource);
    }
}