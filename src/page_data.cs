using System.Data;
using System.Text;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DmgScy;

public class PageData {
    public string fileLoc;
    public string html;

    public PageData(string fileLoc){
        this.fileLoc = fileLoc;
        this.html = String.Concat(File.ReadAllLines(fileLoc));
    }

    private string ConvertTableToHTML(DataTable dataTable){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<table>");
        foreach(DataRow row in dataTable.Rows){
            stringBuilder.Append("<tr>");
            foreach(DataColumn column in dataTable.Columns){
                stringBuilder.Append($"<td>{row[column.ColumnName]}</td>");
            }
            stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("</table>");
        return stringBuilder.ToString();
    }

    private string InsertTableIntoHTML(DataTable dataTable){
        string tableElement = ConvertTableToHTML(dataTable);
        string newHtml = html.Replace(Constants.Html.insertionMarker, tableElement);
        return newHtml;
    }

    public void WriteNewHTML(string outfileLoc, DataTable dataTable){
        string outContent = InsertTableIntoHTML(dataTable);
        File.WriteAllText(outfileLoc, outContent);
    }


}

