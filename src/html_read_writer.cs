using System.Data;
using System.Text;
using System.Web;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using OneOf;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Drawing;

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
    string header;

    public HtmlWriter(DataServiceManager dataServiceManager, string fileLoc, string header): base(fileLoc){
        this.dataServiceManager = dataServiceManager;
        this.header = header;        
    }

    private void HandleTableColumns(StringBuilder stringBuilder, DataServiceManager dataServiceManager, DataRow row){
        if(dataServiceManager.dataService.IsT0){
            stringBuilder.AppendLine("<tr>");
            object bandNameObj = row["name"];
            string bandName = $"{bandNameObj}";
            string bandNameUrl = HttpUtility.UrlEncode(bandName);
            stringBuilder.Append($"<td><a href = \"{bandNameUrl}\">{bandName}</a></td>");
            stringBuilder.AppendLine("\n</tr>");
        }
        if(dataServiceManager.dataService.IsT1){
            object itemName = row["name"];
            object itemUrl = row["url"];
            object itemPrice = row["price"];
            object itemImage = row["image"];
            stringBuilder.AppendLine($"<table style=\"float: left;\"><td><a href = \"{itemUrl}\"><img src=\"data: image / png; base64, {itemImage} \" width=\"206\" height=\"300\" alt=\"{itemName}\"></a></td>");
            stringBuilder.AppendLine($"<tr><th>{itemPrice}</th></tr></table>");
        } 
    }

    public string ConvertTableToHTML(DataTable dataTable){
        StringBuilder stringBuilder = new StringBuilder();
        if(dataServiceManager.dataService.IsT0){
        stringBuilder.AppendLine("\n<table>");
        }
        foreach(DataRow row in dataTable.Rows){
            HandleTableColumns(stringBuilder, dataServiceManager, row);
        }
        if(dataServiceManager.dataService.IsT0){
        stringBuilder.AppendLine("</table>");
        }
        return stringBuilder.ToString();
    }

    public string InsertTableIntoHTML(string tableAsHtml){
        string newHtml = html.Replace(Constants.Html.tableMarker, tableAsHtml);
        newHtml = newHtml.Replace(Constants.Html.titleMarker, header);
        return newHtml;
    }

    public void WriteNewHTML(string outfileLoc, string outHtml){
        File.WriteAllText(outfileLoc, outHtml);
    }
}

