namespace DmgScy;

public class PageData {
    public string fileLoc;
    public string html;

    public PageData(string fileLoc){
        this.fileLoc = fileLoc;
        this.html = String.Concat(File.ReadAllLines(fileLoc));
    }

    public void InjectSqlData(){
        
    }
}

