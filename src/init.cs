using System.Text;

namespace DmgScy;

public static class Init{

    public static void RequiredFiles(){
        if(!Directory.Exists(Constants.InternalStorage.appDirectory)){
            Directory.CreateDirectory(Constants.InternalStorage.appDirectory);
        }
        if(!Directory.Exists(Constants.InternalStorage.databaseSubdir)){
            Directory.CreateDirectory(Constants.InternalStorage.databaseSubdir);
        }
        if(!Directory.Exists(Constants.InternalStorage.pagedataSubdir)){
            Directory.CreateDirectory(Constants.InternalStorage.pagedataSubdir);
        }
        if(!File.Exists(Constants.Html.Templates.indexBase)){
            using(FileStream filestream = File.Create(Constants.Html.Templates.indexBase)){
                string data = Constants.Html.Templates.Init.index;
                byte[] bytes = new UTF8Encoding(true).GetBytes(data);
                filestream.Write(bytes, 0, bytes.Length);
            }
        }
        if(!File.Exists(Constants.Html.Templates.collectionBase)){
            using(FileStream filestream = File.Create(Constants.Html.Templates.collectionBase)){
                string data = Constants.Html.Templates.Init.collection;
                byte[] bytes = new UTF8Encoding(true).GetBytes(data);
                filestream.Write(bytes, 0, bytes.Length);
            }
        }
        if(!File.Exists(Constants.Html.Templates.shutdown)){
            using(FileStream filestream = File.Create(Constants.Html.Templates.shutdown)){
                string data = Constants.Html.Templates.Init.shutdown;
                byte[] bytes = new UTF8Encoding(true).GetBytes(data);
                filestream.Write(bytes, 0, bytes.Length);
            }
        }        
    }
}