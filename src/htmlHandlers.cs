using HtmlAgilityPack;
using System.Drawing;
using System.Data;
using System.Text;
using System.Web;

namespace DmgScy;

class GetHTML {
    private HtmlWeb web;
    private HtmlAgilityPack.HtmlDocument document;

    public GetHTML(string url){
        this.web = new HtmlWeb();
        this.document = web.Load(url);
    }

    public IList<HtmlNode> ReturnNodes(string css_selector){
        var html_nodes = document.DocumentNode.QuerySelectorAll(css_selector);
        return html_nodes;
    }

    public static HtmlNode ReturnSubNode(HtmlNode parent, string selector){
        var sub_node = parent.SelectSingleNode($".//{selector}");
        return sub_node;
        }
    
    public static string ReturnInnerText(HtmlNode sub_node){
        return HtmlEntity.DeEntitize(sub_node.InnerText).Trim();
    }

    public static string ReturnValue(HtmlNode sub_node, string attribute){
        return HtmlEntity.DeEntitize(sub_node.Attributes[attribute].Value);
    }
}

class HtmlParser{
    public IList<HtmlNode> nodes;

    public HtmlParser(IList<HtmlNode> nodes){
        this.nodes = nodes;
    }

    public List<Band> ParseBands(){
        List<Band> bandList = new List<Band>();
        foreach(HtmlNode node in nodes){
            HtmlNode link_subnode = GetHTML.ReturnSubNode(node, Constants.band_link_selector);
            string name = GetHTML.ReturnInnerText(link_subnode);
            string url = GetHTML.ReturnValue(link_subnode, Constants.url_attribute);
            Band band = new Band(name: name, url: $"{Constants.base_url}{url}");
            bandList.Add(band);
            }
        return bandList;
    }

    public List<Collection> ParseCollection(){
        List<Collection> itemList = new List<Collection>();
        foreach(var node in nodes){
            HtmlNode link_subnode = GetHTML.ReturnSubNode(node, Constants.coll_link_selector);
            string name = GetHTML.ReturnInnerText(link_subnode);
            string url = GetHTML.ReturnValue(link_subnode, Constants.url_attribute);
            HtmlNode price_subnode = GetHTML.ReturnSubNode(node, Constants.coll_price_selector);
            string price = GetHTML.ReturnInnerText(price_subnode);
            HtmlNode image_subnode = GetHTML.ReturnSubNode(node, Constants.coll_image_selector);
            string imageSource = GetHTML.ReturnValue(image_subnode, Constants.coll_image_attribute);
            UrlToImage imageConverter = new UrlToImage(imageSource);
            string image = imageConverter.image;
            Collection collection = new Collection(name: name, url: $"{Constants.base_url}{url}", image: image, price: price);
            itemList.Add(collection);
        }
        return itemList;
    }
}

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
    FavouritesHandler favouritesHandler;
    string header;

    public HtmlWriter(DataServiceManager dataServiceManager, string fileLoc, string header): base(fileLoc){
        this.dataServiceManager = dataServiceManager;
        this.header = header;        
        this.favouritesHandler = new FavouritesHandler();
    }

    private void HandleTableColumns(StringBuilder stringBuilder, DataServiceManager dataServiceManager, DataRow row){
        if(dataServiceManager.dataService.IsT0){
            stringBuilder.AppendLine("<tr>");
            object bandNameObj = row["name"];
            string bandName = $"{bandNameObj}";
            string? favIcon = favouritesHandler.SelectState(bandName).Rows[0][0].ToString();
            string bandNameUrl = HttpUtility.UrlEncode(bandName);
            stringBuilder.Append($"<td><a href = \"{Constants.Html.favMarkerUrl}{bandNameUrl}\"><img src=\"data: image / png; base64, {favIcon} \" width=\"15\" height=\"15\"></a></td>");
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
        stringBuilder.AppendLine("\n<table style=\"float: left\"><tr><th colspan=\"2\">All Bands</th></tr>");
        }
        foreach(DataRow row in dataTable.Rows){
            HandleTableColumns(stringBuilder, dataServiceManager, row);
        }
        if(dataServiceManager.dataService.IsT0){
            stringBuilder.AppendLine("</table><table style=\"float: left\"><tr><th>Favourites</th><tr>");
            foreach(DataRow row in favouritesHandler.SelectFavourited().Rows){
                stringBuilder.AppendLine("<tr>");
                object bandNameObj = row["name"];
                string bandName = $"{bandNameObj}";
                string bandNameUrl = HttpUtility.UrlEncode(bandName);
                stringBuilder.Append($"<td><a href = \"{bandNameUrl}\">{bandName}</a></td>");
                stringBuilder.AppendLine("\n</tr>");                               
            }
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