using System.Data;
using OneOf;

namespace DmgScy;

public static class Run{
    
    public static void Main(){
        //initiate services
        BandService bandService = new BandService();
        DataServiceManager dataServiceManager = new DataServiceManager(bandService);
        HtmlWriter indexWriter = new HtmlWriter(fileLoc: Constants.Html.indexBase, dataServiceManager: dataServiceManager);
        PageService pageService = new PageService(dataServiceManager: dataServiceManager, htmlWriter: indexWriter);

        //collect and create index data by scraping dmgscy
        BandOrCollectionList bandList = pageService.ScrapeWebData(url: Constants.base_url, cssSelector: Constants.band_css_selector);
        pageService.CreatePageData(bandList);

        //read newly-created index data
        HtmlReader indexReader = new HtmlReader(Constants.Html.index);

        //initiate server with the newly-created index data
        Server server = new Server(Constants.localhost, indexReader.html);
        server.listener.Start();
        Console.WriteLine("Listening for connections on {0}", Constants.localhost);
        server.HandleIncomingConnections().GetAwaiter().GetResult();
        server.listener.Close();
    }
}