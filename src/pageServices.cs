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
    public BandOrCollectionService dataService;
    public DataTable? dataTable;

    public PageData(BandOrCollectionService dataService){
        this.dataService = dataService;
    }

    public BandOrCollectionList? ScrapeWebData(string url, string cssSelector){
        GetHTML getHtml = new GetHTML(url);
        IList<HtmlNode> nodes = getHtml.ReturnNodes(cssSelector);
        HtmlParser parseHtml = new HtmlParser(nodes);
        BandOrCollectionList? dataList = null;
        if(dataService.IsT0){
            dataList = parseHtml.ParseBands();
            dataService.AsT0.DatabaseCreate();
            dataService.AsT0.DatabaseInsert(dataList.AsT0);
        }
        else if(dataService.IsT1){
            dataList = parseHtml.ParseCollection();
            dataService.AsT1.DatabaseCreate();
            dataService.AsT1.DatabaseInsert(dataList.AsT1);
        }
        return dataList;
    }

    public DataTable? CreatePageData(HtmlWriter htmlWriter){
        string outfileLoc = "";
        if(dataService.IsT0){
            dataTable = dataService.AsT0.DatabaseSelect();
            outfileLoc = Constants.Html.index;
        }
        else if(dataService.IsT1){
            dataTable = dataService.AsT1.DatabaseSelect();
            outfileLoc = Constants.Html.collectionLast;
        }
        if(dataTable != null){
            string dataTableHtml = htmlWriter.ConvertTableToHTML(dataTable);
            string newHtml = htmlWriter.InsertTableIntoHTML(dataTableHtml);
            htmlWriter.WriteNewHTML(outfileLoc: outfileLoc, newHtml);
        }
        return dataTable;
    }
}

enum PageType{
    INDEX,
    COLLECTION
}

class PageBuilder{
    public PageData pageData;
    public PageType pageType;
    private BandOrCollectionService dataService;
    private HtmlWriter htmlWriter;
    public string pageName;

    public PageBuilder(PageType pageType, string pageName){
        this.pageName = pageName;
        this.pageType = pageType;
        this.dataService = InstantiateDataService();
        this.htmlWriter = InstantiateHtmlWriter();
        this.pageData = new PageData(dataService: dataService);
    }

    private BandOrCollectionService InstantiateDataService(){
        string tableName = StringCleaner.EraseIllegalChars(pageName);
        if(pageType == PageType.INDEX){
            return new BandService(dataBase: Constants.Sql.dataSource, tableName: Constants.Sql.bandsTableName);
        }
        else{
            return new CollectionService(tableName: tableName, dataBase: Constants.Sql.dataSource);
        }
    }

    private HtmlWriter InstantiateHtmlWriter(){
        if(pageType == PageType.INDEX){
            return new HtmlWriter(fileLoc: Constants.Html.indexBase, dataService: dataService, header: pageName);
        }
        else{
            return new HtmlWriter(fileLoc: Constants.Html.collectionBase, dataService: dataService, header: pageName);
        }
    }

    public void Build(string sourceUrl, bool fromLocal = false){
        if(!fromLocal){
            if(pageType == PageType.INDEX){
                pageData.ScrapeWebData(url: sourceUrl, cssSelector: Constants.band_css_selector);
            }
            else if(pageType == PageType.COLLECTION){
                pageData.ScrapeWebData(url: sourceUrl, cssSelector: Constants.collection_css_selector);
            }
        }
        pageData.CreatePageData(htmlWriter);
    }
}
