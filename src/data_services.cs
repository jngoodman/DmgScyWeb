using System.Data;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;

namespace DmgScy;

interface IDataService{
    public string Url {get; set; }
    public string Css_selector {get; set; }
    public GetHTML Get_html {get; set; }
    public IList<HtmlNode> Nodes {get; set; }
}

class BandService: IDataService {
    public string Url{get; set; } = Constants.all_bands_url;
    public string Css_selector{get; set; } = Constants.band_css_selector;
    public GetHTML Get_html {get; set; }
    public IList<HtmlNode> Nodes {get; set; }
    public HandleDatabase databaseHandler {get; set; }

    public BandService(){
        this.Get_html = new GetHTML(Url);
        this.Nodes = Get_html.ReturnNodes(Css_selector);
        this.databaseHandler = new HandleDatabase();        
    }    

    public List<Band> GetBands(){
        var bandList = new List<Band>();
        foreach(var node in Nodes){
            var link_subnode = GetHTML.ReturnSubNode(node, Constants.band_link_selector);
            var name = GetHTML.ReturnInnerText(link_subnode);
            var url = GetHTML.ReturnValue(link_subnode, Constants.url_attribute);
            var band = new Band(name: name, url: $"{Constants.base_url}{url}");
            bandList.Add(band);
        }
        return bandList;
    }

    public void DatabaseCreate(){
        databaseHandler.RunQuery(Constants.Sql.createBands);
    }

    public DataTable? DatabaseSelect(){
        DataTable? dataTable = databaseHandler.RunQuery(Constants.Sql.selectBands, returnTable: true);
        return dataTable;
    }

    public Dictionary<string, DataTable?> DatabaseSelectWithHeader(){
        Dictionary<string, DataTable?> dataTableWithHeader = new Dictionary<string, DataTable?>(){
            {"Bands", DatabaseSelect()}
        };
        return dataTableWithHeader;
    }

    public void DatabaseInsert(){
        List<Band> bandList = GetBands();
        foreach(Band band in bandList){
            List<SqliteParameter> parameterList = new List<SqliteParameter>(){
                new SqliteParameter("@name", band.name),
                new SqliteParameter("@url", band.url)
            };
            databaseHandler.RunQuery(Constants.Sql.addBands, parameters: parameterList);
        }
    }    
}

class CollectionService: IDataService {
    public string Url{get; set; }
    public string Css_selector{get; set; } = Constants.collection_css_selector;
    public GetHTML Get_html {get; set; }
    public IList<HtmlNode> Nodes {get; set; }

    public CollectionService(string url){
        this.Url = url;
        this.Get_html = new GetHTML(url);
        this.Nodes = Get_html.ReturnNodes(Css_selector);
    }    

    public List<Collection> GetCollection(){
        var collection_list = new List<Collection>();
        foreach(var node in Nodes){
            var link_subnode = GetHTML.ReturnSubNode(node, Constants.coll_link_selector);
            var name = GetHTML.ReturnInnerText(link_subnode);
            var url = GetHTML.ReturnValue(link_subnode, Constants.url_attribute);
            var price_subnode = GetHTML.ReturnSubNode(node, Constants.coll_price_selector);
            var price = GetHTML.ReturnInnerText(price_subnode);
            var image_subnode = GetHTML.ReturnSubNode(node, Constants.coll_image_selector);
            var image = GetHTML.ReturnValue(image_subnode, Constants.coll_image_attribute);
            var collection = new Collection { name = name, url = $"{Constants.base_url}{url}", image = image, price = price};
            collection_list.Add(collection);
        }
        return collection_list;
    }    
}