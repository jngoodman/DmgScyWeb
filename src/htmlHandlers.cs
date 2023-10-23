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
            HtmlNode link_subnode = GetHTML.ReturnSubNode(node, Constants.Selectors.bandLinkSelector);
            string name = GetHTML.ReturnInnerText(link_subnode);
            string url = GetHTML.ReturnValue(link_subnode, Constants.Selectors.urlAttribute);
            Band band = new Band(name: name, url: $"{Constants.ExternalUrls.baseUrl}{url}", state: Constants.InternalStorage.Images.notFavourited);
            bandList.Add(band);
            }
        return bandList;
    }

    public List<Collection> ParseCollection(){
        List<Collection> itemList = new List<Collection>();
        foreach(var node in nodes){
            HtmlNode link_subnode = GetHTML.ReturnSubNode(node, Constants.Selectors.collectionLinkSelector);
            string name = GetHTML.ReturnInnerText(link_subnode);
            string url = GetHTML.ReturnValue(link_subnode, Constants.Selectors.urlAttribute);
            HtmlNode price_subnode = GetHTML.ReturnSubNode(node, Constants.Selectors.collectionPriceSelector);
            string price = GetHTML.ReturnInnerText(price_subnode);
            HtmlNode image_subnode = GetHTML.ReturnSubNode(node, Constants.Selectors.collectionImageSelector);
            string imageSource = GetHTML.ReturnValue(image_subnode, Constants.Selectors.collectionImageAttribute);
            UrlToImage imageConverter = new UrlToImage(imageSource);
            string image = imageConverter.image;
            Collection collection = new Collection(name: name, url: $"{Constants.ExternalUrls.baseUrl}{url}", image: image, price: price);
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
    BandOrCollectionService dataService;
    string header;

    public HtmlWriter(BandOrCollectionService dataService, string fileLoc, string header): base(fileLoc){
        this.dataService = dataService;
        this.header = header;
    }

    private void HandleRows(StringBuilder stringBuilder, DataRow row){
        if(dataService.IsT0){
            string rowData = Constants.Html.Builders.indexMainRow;
            rowData = rowData.Replace("{bandName}", $"{row["name"]}");
            rowData = rowData.Replace("{favIcon}", $"{row["state"]}");
            rowData = rowData.Replace("{bandNameUrl}", HttpUtility.UrlEncode($"{row["name"]}"));
            stringBuilder.Append(rowData);
        }
        if(dataService.IsT1){
            string rowData = Constants.Html.Builders.collectionMainRow;
            rowData = rowData.Replace("{itemName}", $"{row["name"]}");
            rowData = rowData.Replace("{itemUrl}", $"{row["url"]}");
            rowData = rowData.Replace("{itemPrice}", $"{row["price"]}");
            rowData = rowData.Replace("{itemImage}", $"{row["image"]}");
            stringBuilder.Append(rowData);
        } 
    }


    public string ConvertTableToHTML(DataTable dataTable){
        StringBuilder stringBuilder = new StringBuilder();
        if(dataService.IsT0){
        stringBuilder.Append(Constants.Html.Builders.indexTableHeader);
        }
        foreach(DataRow row in dataTable.Rows){
            HandleRows(stringBuilder, row);
        }
        if(dataService.IsT0){
            stringBuilder.Append(Constants.Html.Builders.favouritesTableHeader);
            foreach(DataRow row in dataTable.Rows){
                if($"{row["state"]}" == Constants.InternalStorage.Images.favourited){
                    string rowData = Constants.Html.Builders.favouritesRow;
                    rowData = rowData.Replace("{bandName}", $"{row["name"]}");
                    rowData = rowData.Replace("{bandNameUrl}", HttpUtility.UrlEncode($"{row["name"]}"));
                    stringBuilder.Append(rowData);
                }                    
            }
        }
        return stringBuilder.ToString();
    }

    public string InsertTableIntoHTML(string tableAsHtml){
        string newHtml = html.Replace(Constants.Html.Wildcards.tableMarker, tableAsHtml);
        newHtml = newHtml.Replace(Constants.Html.Wildcards.titleMarker, header);
        return newHtml;
    }

    public void WriteNewHTML(string outfileLoc, string outHtml){
        File.WriteAllText(outfileLoc, outHtml);
    }
}