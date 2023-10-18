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