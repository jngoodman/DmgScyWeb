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

    public HandleDatabase(){
        this.connectionString = Constants.Sql.dataSource;
        this.dbConnection = new SqliteConnection(connectionString);
    }
        
    private DataTable? ExecuteCommand(SqliteCommand command, bool returnTable = false){
        if(!returnTable){
            Console.WriteLine("Test");
            command.ExecuteNonQuery();
            Console.WriteLine("Test");
            return null;
        }       
        DataTable dataTable = new DataTable();
        SqliteDataReader reader = command.ExecuteReader();
        for(int columnNumber = 0; columnNumber < reader.FieldCount; columnNumber++){
            DataColumn column = new DataColumn(reader.GetName(columnNumber));
            dataTable.Columns.Add(column);
            }
        while(reader.Read()){
            int rowNumber = 0;
            DataRow row = dataTable.NewRow();
            dataTable.Rows.Add(row);
            for(int columnNumber = 0; columnNumber < reader.FieldCount; columnNumber++){
                dataTable.Rows[rowNumber][columnNumber] = reader.GetValue(columnNumber);
            }
            rowNumber++;
         }
        return dataTable;
    }
    
    private void InsertParams(SqliteCommand command, List<string>? parameters = null, bool returnTable = false){
        if(parameters != null){
            for(int paramNumber = 0; paramNumber < parameters.Count; paramNumber ++){
                string positionIdentifier = $"@{paramNumber}";
                command.Parameters.Add(new SqliteParameter(positionIdentifier, parameters[paramNumber]));
            }
        } 
    }     

    public DataTable? RunQuery(string commandString, List<string>? parameters = null, bool returnTable = false){
        //Will return a DataTable ready for Html injection only if RETURN is a specified option. See HandleDatabase.ExecuteCommand();
        SqliteCommand command = new SqliteCommand(commandString, dbConnection);
        InsertParams(command, parameters, returnTable);
        dbConnection.Open();
        Console.WriteLine(commandString);
        DataTable? dataTable = ExecuteCommand(command, returnTable);
        dbConnection.Close();
        return dataTable;
    }
}