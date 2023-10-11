using System.Data.Common;
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
        
    private List<string>? ExecuteCommand(SqliteCommand command, List<Options> options){
        if(!options.Contains(Options.RETURN)){
            command.ExecuteNonQuery();
            return null;
        }       
        List<string> returnList = new List<string>();         
        SqliteDataReader reader = command.ExecuteReader();
        while(reader.Read()){
            foreach(int columnNumber in reader){
                returnList.Add(reader.GetString(columnNumber));
            }
        }
        return returnList;
    }
    
    private void InsertParams(SqliteCommand command, List<string>? parameters, List<Options> options){
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

    public List<string>? RunQuery(string commandString, List<Options> options, List<string>? parameters){
        SqliteCommand command = new SqliteCommand(commandString, dbConnection);
        InsertParams(command, parameters, options);
        dbConnection.Open();
        var returnList = ExecuteCommand(command, options);
        dbConnection.Close();
        return returnList;
    }
}