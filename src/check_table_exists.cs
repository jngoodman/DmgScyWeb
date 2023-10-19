using Microsoft.Data.Sqlite;

namespace DmgScy;

public static class TableExists{

    public static bool Check(string tableName){
        HandleDatabase databaseHandler = new HandleDatabase();
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