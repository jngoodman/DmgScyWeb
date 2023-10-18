using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OneOf.Types;

namespace DmgScy;
public class DataServiceManager{
    public BandOrCollectionService  dataService;
    public DataServiceManager(BandOrCollectionService dataService){
        this.dataService = dataService;
    }
    
    public void CreateTable(){        
        if(dataService.IsT0){
            dataService.AsT0.DatabaseCreate();
        }
        else if(dataService.IsT1){
            dataService.AsT1.DatabaseCreate();
        }
    }

    public void FillTable(BandOrCollectionList insertList){        
        try{
            if(dataService.IsT0){
                dataService.AsT0.DatabaseInsert(insertList.AsT0);
            }
            else if(dataService.IsT1){
                dataService.AsT1.DatabaseInsert(insertList.AsT1);
            }
        }
        catch(SqliteException){
            Console.WriteLine("Sqlite error. Check insertList matches dimensions of table.");
        }
    }

    public DataTable SelectTable(){
        DataTable dataTable = new DataTable();
        try{
            if(dataService.IsT0){
                dataTable = dataService.AsT0.DatabaseSelect();
            }
            else if(dataService.IsT1){
                dataTable = dataService.AsT1.DatabaseSelect();
            }
        }
        catch(SqliteException){
            Console.WriteLine("Sqlite error. Check table exists. Returning null.");
        }
        return dataTable;
    }

    public DataTable GetDataTable(BandOrCollectionList insertList){
        CreateTable();
        FillTable(insertList);
        return SelectTable();
    } 
}

public static class StringCleaner{

    public static string EraseIllegalChars(string inputString){
        inputString = inputString.ToLower();
        List<string> outputList = new List<string>();
        foreach(char character in inputString){
            if(Constants.legalTableCharacters.Contains(character)){
                outputList.Add(character.ToString());
            }
        }
        string outputString = string.Join("", outputList);
        return outputString;        
    }
}
