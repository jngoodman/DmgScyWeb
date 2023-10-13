using System.Net.NetworkInformation;

namespace DmgScy;

static class Constants {
    public static string base_url= "https://damagedsociety.co.uk";
    public static string all_bands_url = "https://damagedsociety.co.uk/pages/band-a-z";
    public static string band_css_selector = "div.instant-brand-item.vertical.desk_left.mob_center";
    public static string band_link_selector = "a[@class='instant-brand-text-link']";
    public static string collection_css_selector = "div.card.card--with-hover.column.third";
    public static string coll_link_selector = "a[@class='card__link']";
    public static string coll_price_selector = "span[@class='product-price__amount theme-money']";
    public static string coll_image_selector = "div[@class='prod-image__main lazyload-placeholder']//img";
    public static string coll_image_attribute = "data-src";
    public static string url_attribute = "href";
    public static string localhost = "http://localhost:8000/";
    
    public static class Sql {
        public static string dataSource = "Data Source=\"src/dmgscy.db\"";
        public static string createBands = "CREATE TABLE IF NOT EXISTS bands (name TEXT NOT NULL, url TEXT NOT NULL, UNIQUE(name));";
        public static string addBands = "INSERT OR IGNORE INTO bands (name, url) VALUES (@0, @1);";
        public static string selectBands = "SELECT * FROM bands;";
        public static string createCollection = "CREATE TABLE IF NOT EXISTS {collectionName} (name TEXT NOT NULL, url TEXT NOT NULL, UNIQUE(name));";
        public static string addCollection = "INSERT OR IGNORE INTO {collectionName} (name, url, image, price) VALUES (@0, @1, @2, @3);";
        public static string selectCollection = "SELECT * FROM {collectionName};";
    }

    public static class Html {
        public static string indexBase = "src/pagedata/index_base.html";
        public static string collectionBase = "src/pagedata/collection_base.html";
        public static string insertionMarker = "##TableMarker##";
        public static string index = "src/pagedata/index.html";
        public static string collection = "src/pagedata/collection.html";
    }
}