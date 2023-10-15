using System.Data;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace DmgScy;

public class PageData {
    public string fileLoc;
    public string html;

    public PageData(string fileLoc){
        this.fileLoc = fileLoc;
        this.html = String.Concat(File.ReadAllLines(fileLoc));
    }

    private void HandleTableColumns(DataTable dataTable, StringBuilder stringBuilder, DataRow row, string tableHeader){
        if(tableHeader == "Bands"){
            object bandNameObj = row["name"];
            string bandName = $"{bandNameObj}";
            StringBuilder urlString = new StringBuilder(bandName);
            urlString.Replace(" ", "");
            urlString.Replace("/", "");
            string bandNameUrl = urlString.ToString().ToLower();
            stringBuilder.Append($"<td><a href = \"{bandNameUrl}\">{bandName}</a></td>");
        } 
    }

    private string ConvertTableToHTML(DataTable dataTable, string tableHeader){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\n<table>");
        foreach(DataRow row in dataTable.Rows){
            stringBuilder.AppendLine("<tr>");
            HandleTableColumns(dataTable, stringBuilder, row, tableHeader);
            stringBuilder.AppendLine("\n</tr>");
        }
        stringBuilder.AppendLine("</table>");
        return stringBuilder.ToString();
    }

    private string InsertTableIntoHTML(DataTable dataTable, string tableHeader){
        string tableElement = ConvertTableToHTML(dataTable, tableHeader);
        string newHtml = html.Replace(Constants.Html.insertionMarker, tableElement);
        return newHtml;
    }

    public void WriteNewHTML(string outfileLoc, DataTable dataTable, string tableHeader){
        string outContent = InsertTableIntoHTML(dataTable, tableHeader);
        File.WriteAllText(outfileLoc, outContent);
    }
}

