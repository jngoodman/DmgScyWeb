using System.Data;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using OneOf;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace DmgScy;

public class HtmlReader {
    public string fileLoc;
    public string html;

    public HtmlReader(string fileLoc){
        this.fileLoc = fileLoc;
        this.html = String.Concat(File.ReadAllLines(fileLoc));
    }
}

public class HtmlWriter: HtmlReader {
    DataServiceManager dataServiceManager;

    public HtmlWriter(DataServiceManager dataServiceManager, string fileLoc): base(fileLoc){
        this.dataServiceManager = dataServiceManager;
    }

    private void HandleTableColumns(StringBuilder stringBuilder, DataServiceManager dataServiceManager, DataRow row){
        if(dataServiceManager.dataService.IsT0){
            object bandNameObj = row["name"];
            string bandName = $"{bandNameObj}";
            StringBuilder urlString = new StringBuilder(bandName);
            urlString.Replace(" ", "");
            urlString.Replace("/", "");
            string bandNameUrl = urlString.ToString().ToLower();
            stringBuilder.Append($"<td><a href = \"{bandNameUrl}\">{bandName}</a></td>");
        } 
    }

    public string ConvertTableToHTML(DataTable dataTable){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\n<table>");
        foreach(DataRow row in dataTable.Rows){
            stringBuilder.AppendLine("<tr>");
            HandleTableColumns(stringBuilder, dataServiceManager, row);
            stringBuilder.AppendLine("\n</tr>");
        }
        stringBuilder.AppendLine("</table>");
        return stringBuilder.ToString();
    }

    public string InsertTableIntoHTML(string tableAsHtml){
        string newHtml = html.Replace(Constants.Html.insertionMarker, tableAsHtml);
        return newHtml;
    }

    public void WriteNewHTML(string outfileLoc, string outHtml){
        File.WriteAllText(outfileLoc, outHtml);
    }
}

