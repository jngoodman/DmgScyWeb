using System.Net.NetworkInformation;
using OneOf;

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
    public static string legalTableCharacters = "abcdefghijklmnopqrstuvwxyz";
    public static string imageRightPartEncoder = "?v=";
    public static int refreshDays = 7;
    
    public static class Sql {
        public static string dataSource = "src/dmgscy.db";
        public static string faveSource = "src/favourites.db";
        public static string createBands = "CREATE TABLE IF NOT EXISTS {tableName} (name TEXT NOT NULL, url TEXT NOT NULL, UNIQUE(name));";
        public static string addBands = "INSERT OR IGNORE INTO {tableName} (name, url) VALUES (@name, @url);";
        public static string select = "SELECT * FROM {tableName};";
        public static string createCollection = "CREATE TABLE IF NOT EXISTS {tableName} (name TEXT NOT NULL, url TEXT NOT NULL, image TEXT NOT NULL, price TEXT NOT NULL, UNIQUE(name));";
        public static string addCollection = "INSERT OR IGNORE INTO {tableName} (name, url, image, price) VALUES (@name, @url, @image, @price);";
    }

    public static class Html {
        public static string indexBase = "pagedata/index_base.html";
        public static string collectionBase = "pagedata/collection_base.html";
        public static string index = "pagedata/index.html";
        public static string collectionLast = "pagedata/last_collection.html";
        public static string shutdown = "pagedata/shutdown.html";
        public static string shutdownCommand = "/shutdown";
        public static string tableMarker = "##TableMarker##";
        public static string titleMarker = "##TitleMarker##";
    }

    public static string favouriteIconBase64 = "iVBORw0KGgoAAAANSUhEUgAAABUAAAAVCAYAAACpF6WWAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAE/SURBVDhPrZPRagIxEEVntW4LtuBL34T+Qv//b9qXgkihBUFd1/TeZKJJTGIKe8CdJDO5mWRG+QcrtZNi1N5lpnZSqqJdpwNkaT6ctd87VEVNIDGMOmjgmksZl+UjfgdseLNr1X1Nb3o643NU20DuxPDdjsiylzlGvL5azTYk0uEkenwtSJER2c6T+6WH+BPsu+32Iv1CZPGAFb8xVyBmTHDAcMJ1BpHl00U8aBpfEAoisBmNDwsYvQVwwsyypSgal3ZEKkqcMD21Vld/KkhyLdUx8GuDUc5LsE5/TpCUtrliVaj5i6Ivz/j46zMXVtznhHXrL5ATNeYT/8oeIxXbfmO4dtaL0689ffPyqaibM4yi7AD06es7F6WzFvOfX1j6r0Q6N48MXPWBFoKEcTazJCbSyYqqJTm/pzVuCkT+AF6CVURB95a8AAAAAElFTkSuQmCC";
}