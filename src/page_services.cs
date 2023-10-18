using System.Data;
using System.Text;
using OneOf;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.Features;

namespace DmgScy;

class PageData{
    public HtmlWriter htmlWriter;
    public DataServiceManager dataServiceManager;
    public BandOrCollectionList? dataList;

    public PageData(HtmlWriter htmlWriter, DataServiceManager dataServiceManager){
        this.htmlWriter = htmlWriter;
        this.dataServiceManager = dataServiceManager;
    }

    public BandOrCollectionList ScrapeWebData(string url, string cssSelector){
        GetHTML getHtml = new GetHTML(url);
        IList<HtmlNode> nodes = getHtml.ReturnNodes(cssSelector);
        HtmlParser parseHtml = new HtmlParser(nodes);
        if(dataServiceManager.dataService.IsT0){
            dataList = parseHtml.ParseBands();
        }
        else{
            dataList = parseHtml.ParseCollection();
        }    
        return dataList;    
    }

    public void CreatePageData(BandOrCollectionList insertList){
        DataTable dataTable = dataServiceManager.GetDataTable(insertList);
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

enum PageType{
    INDEX,
    COLLECTION
}

class PageBuilder{
    public PageData pageData;
    public PageType pageType;
    private DataServiceManager dataServiceManager;
    private HtmlWriter htmlWriter;

    public PageBuilder(PageType pageType, string pageName){
        this.pageType = pageType;
        this.dataServiceManager = InstantiateDataServiceManager(pageName);
        this.htmlWriter = InstantiateHtmlWriter();
        this.pageData = new PageData(dataServiceManager: dataServiceManager, htmlWriter: htmlWriter);
    }

    private DataServiceManager InstantiateDataServiceManager(string pageName){
        string tableName = StringCleaner.EraseIllegalChars(pageName);
        if(pageType == PageType.INDEX){
            return new DataServiceManager(new BandService(tableName: tableName));
        }
        else{
            return new DataServiceManager(new CollectionService(tableName: tableName));
        }
    }

    private HtmlWriter InstantiateHtmlWriter(){
        if(pageType == PageType.INDEX){
            return new HtmlWriter(fileLoc: Constants.Html.indexBase, dataServiceManager: dataServiceManager);
        }
        else{
            return new HtmlWriter(fileLoc: Constants.Html.collectionBase, dataServiceManager: dataServiceManager);
        }
    }

    public void Build(string sourceUrl){
        if(pageType == PageType.INDEX){
            BandOrCollectionList bandList = pageData.ScrapeWebData(url: sourceUrl, cssSelector: Constants.band_css_selector);
            pageData.CreatePageData(bandList);
        }
        else if(pageType == PageType.COLLECTION){
            BandOrCollectionList itemList = pageData.ScrapeWebData(url: sourceUrl, cssSelector: Constants.collection_css_selector);
            pageData.CreatePageData(itemList);
        }
    }
}
