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
            stringBuilder.Append($"<td><a href=\"{Constants.localhost}collection/{row["name"]}\">{row["name"]}</a></td>");
        } 
    }

    private string ConvertTableToHTML(DataTable dataTable, string tableHeader){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<table>");
        foreach(DataRow row in dataTable.Rows){
            stringBuilder.Append("<tr>");
            HandleTableColumns(dataTable, stringBuilder, row, tableHeader);
            stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("</table>");
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

