namespace DmgScy;
using OneOf;
using HtmlAgilityPack;

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
            string image = GetHTML.ReturnValue(image_subnode, Constants.coll_image_attribute);
            Collection collection = new Collection { name = name, url = $"{Constants.base_url}{url}", image = image, price = price};
            itemList.Add(collection);
        }
        return itemList;
    }
}