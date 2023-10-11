using System.Net;
using System.Reflection.Metadata;
using HapCss;
using HtmlAgilityPack;
using Microsoft.VisualBasic;

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