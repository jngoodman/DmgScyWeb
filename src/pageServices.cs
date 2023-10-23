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
    public DataTable? dataTable;

    public PageData(HtmlWriter htmlWriter, DataServiceManager dataServiceManager){
        this.htmlWriter = htmlWriter;
        this.dataServiceManager = dataServiceManager;
    }

    public void ScrapeWebData(string url, string cssSelector){
        GetHTML getHtml = new GetHTML(url);
        IList<HtmlNode> nodes = getHtml.ReturnNodes(cssSelector);
        HtmlParser parseHtml = new HtmlParser(nodes);
        BandOrCollectionList dataList;
        if(dataServiceManager.dataService.IsT0){
            dataList = parseHtml.ParseBands();
            FavouritesHandler favouritesHandler = new FavouritesHandler();
            favouritesHandler.CreateFavourites();
            favouritesHandler.InsertStates(dataList.AsT0);
        }
        else{
            dataList = parseHtml.ParseCollection();
        }
        dataServiceManager.GenDataTable(dataList);  
    }

    public DataTable? CreatePageData(){
        dataTable = dataServiceManager.SelectTable();
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
    private DataServiceManager dataServiceManager;
    private HtmlWriter htmlWriter;
    public string pageName;

    public PageBuilder(PageType pageType, string pageName){
        this.pageName = pageName;
        this.pageType = pageType;
        this.dataServiceManager = InstantiateDataServiceManager();
        this.htmlWriter = InstantiateHtmlWriter();
        this.pageData = new PageData(dataServiceManager: dataServiceManager, htmlWriter: htmlWriter);
    }

    private DataServiceManager InstantiateDataServiceManager(){
        string tableName = StringCleaner.EraseIllegalChars(pageName);
        if(pageType == PageType.INDEX){
            return new DataServiceManager(new BandService(dataBase: Constants.Sql.dataSource));
        }
        else{
            return new DataServiceManager(new CollectionService(tableName: tableName, dataBase: Constants.Sql.dataSource));
        }
    }

    private HtmlWriter InstantiateHtmlWriter(){
        if(pageType == PageType.INDEX){
            return new HtmlWriter(fileLoc: Constants.Html.indexBase, dataServiceManager: dataServiceManager, header: pageName);
        }
        else{
            return new HtmlWriter(fileLoc: Constants.Html.collectionBase, dataServiceManager: dataServiceManager, header: pageName);
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
        pageData.CreatePageData();
    }
}
