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

    public enum Options{
        PARAMS,
        RETURN,
    }   
        
    private DataTable? ExecuteCommand(SqliteCommand command, List<Options>? options = null){
        if(options != null){
            if(!options.Contains(Options.RETURN) ){
                command.ExecuteNonQuery();
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
        return null;
    }
    
    private void InsertParams(SqliteCommand command, List<string>? parameters = null, List<Options>? options = null){
        if(options != null){
            if(options.Contains(Options.PARAMS) && parameters != null){
                foreach(var parameter in parameters){
                        command.Parameters.Add(parameter);
                }
            }
            else if(options.Contains(Options.PARAMS) && parameters == null){
                Console.WriteLine("Warning: PARAMS in Options but no parameters provided. Ignoring PARAMS.");
            }
            else if(!options.Contains(Options.PARAMS) && parameters != null){
                Console.WriteLine("Warning: PARAMS not in Options but parameters provided. Ignoring parameters.");
            }
        } 
    }     

    public DataTable? RunQuery(string commandString, List<Options>? options = null, List<string>? parameters = null){
        //Will return a DataTable ready for Html injection only if RETURN is a specified option. See HandleDatabase.ExecuteCommand();
        SqliteCommand command = new SqliteCommand(commandString, dbConnection);
        InsertParams(command, parameters, options);
        dbConnection.Open();
        Console.WriteLine(commandString);
        DataTable? dataTable = ExecuteCommand(command, options);
        dbConnection.Close();
        return dataTable;
    }
}