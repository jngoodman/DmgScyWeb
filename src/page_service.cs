using System.Data;
using OneOf;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;

namespace DmgScy;

class PageService{
    public HtmlWriter htmlWriter;
    public DataServiceManager  dataServiceManager;

    public PageService(HtmlWriter htmlWriter, DataServiceManager dataServiceManager){
        this.htmlWriter = htmlWriter;
        this.dataServiceManager = dataServiceManager;
    }

    public BandOrCollectionList ScrapeWebData(string url, string cssSelector){
        GetHTML getHtml = new GetHTML(url);
        IList<HtmlNode> nodes = getHtml.ReturnNodes(cssSelector);
        HtmlParser parseHtml = new HtmlParser(nodes);
        if(dataServiceManager.dataService.IsT0){
            return parseHtml.ParseBands();
        }
        else{
            return parseHtml.ParseCollection();
        }        
    }

    public void CreatePageData(BandOrCollectionList insertList){
        DataTable? dataTable = dataServiceManager.GetDataTable(insertList);
        if(dataTable != null){
            string dataTableHtml = htmlWriter.ConvertTableToHTML(dataTable);
            string newHtml = htmlWriter.InsertTableIntoHTML(dataTableHtml);
            if(dataServiceManager.dataService.IsT0){
                htmlWriter.WriteNewHTML(outfileLoc: Constants.Html.index, newHtml);
            }
            else if(dataServiceManager.dataService.IsT1){
                htmlWriter.WriteNewHTML(outfileLoc: Constants.Html.collectionLast, newHtml);
            }
        }
    }
}

