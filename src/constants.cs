using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using OneOf;

namespace DmgScy;

static class Constants {
    public static class ExternalUrls{
        public static string baseUrl= "https://damagedsociety.co.uk";
        public static string allBandsUrl = "https://damagedsociety.co.uk/pages/band-a-z";
        public static string imageRightPartEncoder = "?v=";
    }

    public static class InternalUrls{
        public static List<string> indexUrls = new List<string>(){"http://localhost:8000/", "http://localhost:8000/favicon.ico"};
        public static string favouritesSplitter = "&favourite&";
        public static string favouritesCommand = "/favourites";
        public static string shutdownCommand = "/shutdown";
    }

    public static class Selectors{
        public static string bandCssSelector = "div.instant-brand-item.vertical.desk_left.mob_center";
        public static string bandLinkSelector = "a[@class='instant-brand-text-link']";
        public static string collectionCssSelector = "div.card.card--with-hover.column.third";
        public static string collectionLinkSelector = "a[@class='card__link']";
        public static string collectionPriceSelector = "span[@class='product-price__amount theme-money']";
        public static string collectionImageSelector = "div[@class='prod-image__main lazyload-placeholder']//img";
        public static string collectionImageAttribute = "data-src";
        public static string urlAttribute = "href";
    }

    public static class InternalStorage{
        public static int refreshHours = 168;
        public static string dirName = "\\DMGSCYWebscraper";
        public static string appDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + dirName;
        public static string databaseSubdir = appDirectory + "\\database";
        public static string pagedataSubdir = appDirectory + "\\pagedata";
        public static string refreshToken = databaseSubdir + "\\refreshtoken.file";
        public static string dataBase = databaseSubdir + "\\dmgscy.db";

        public static class Tables{
            public static string bands = "Bands";
            public static string favourites = "Favourites";
            public static string legalCharacters = "abcdefghijklmnopqrstuvwxyz";
        }

        public static class SqlCommands {
            public static string createBands = "CREATE TABLE IF NOT EXISTS {tableName} (name TEXT NOT NULL, url TEXT NOT NULL, state TEXT NOT NULL, UNIQUE(name));";
            public static string addBands = "INSERT OR IGNORE INTO {tableName} (name, url, state) VALUES (@name, @url, @state);";
            public static string selectFrom = "SELECT {targetColumn} FROM {tableName} WHERE {conditionColumn} = '{condition}';";
            public static string updateValue = "UPDATE {tableName} SET {targetColumn} = '{newValue}' WHERE {conditionColumn} = '{condition}'";
            public static string select = "SELECT * FROM {tableName};";
            public static string createCollection = "CREATE TABLE IF NOT EXISTS {tableName} (name TEXT NOT NULL, url TEXT NOT NULL, image TEXT NOT NULL, price TEXT NOT NULL, UNIQUE(name));";
            public static string addCollection = "INSERT OR IGNORE INTO {tableName} (name, url, image, price) VALUES (@name, @url, @image, @price);";
            public static string dropTable = "DROP TABLE IF EXISTS {tableName}";
            public static string renameTable = "ALTER TABLE {tableName} RENAME TO {newName}";
        }

        public static class Images {           
            public static string favourited = "iVBORw0KGgoAAAANSUhEUgAAABUAAAAVCAYAAACpF6WWAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsEAAA7BAbiRa+0AAAFCSURBVDhPrZPBbsIwEETXBdKqwKGH3irxC/3/v2kvlaqqFUhIiEDpjL0bEicxi+iTEsfZzWS8a4cTEAchhCek/ui0yDWi4kyVOx3/FZdodPmWRg9up/VRHxxcFDWXs0dxu3U5PfzittfRQa/7mZM93FUywROXr2NYMXQm3xVU6LzhEksc4XaSrS//SXRqddvuRKoZ6jdFxD4cahAdE/ygPmA5tcj8IYnTdbN8ExYKItGN5psg6dS0EaZLT1M0ry1IOtVhINaHgpd2DuMDgiQr+Vn44xOTXlTBe8aHBMnYZ6lZBUrxUdHlAjczwaWy41YSvI/xEXqisVnvIvcVJ7gg9vWNx5c0mjjjbGp2WCIdUSSkHUiHFGXDsE+fX1PtOHK+3mhcab5TBo+pnaq4E0A7xZy1c/JmFc9+ntymlNcTvR2RP9dBlS9oim88AAAAAElFTkSuQmCC";
            public static string notFavourited = favourited + "\" class=\"unfavourited\"";
        }
    }

    public static class Html {
        
        public static class Templates {
            public static string indexBase = InternalStorage.pagedataSubdir + "\\index_base.html";
            public static string collectionBase = InternalStorage.pagedataSubdir+ "\\collection_base.html";
            public static string index = InternalStorage.pagedataSubdir + "\\index.html";
            public static string collection = InternalStorage.pagedataSubdir + "\\collection.html";
            public static string shutdown = InternalStorage.pagedataSubdir + "\\shutdown.html";

            public static class Init {
                public static string index = "<!DOCTYPE html>\n<link rel=\"icon\" href=\"data:,\">\n<meta charset=\"utf-8\">\n<style>\n.unfavourited{\n opacity: 0.33 \n}\nbutton{\nbackground: transparent; \n border: none;\n}\n</style>\n<body>\n<form style=\"float: right\" method=\"post\" action=\"shutdown\">\n<input type=\"submit\" value=\"Shutdown\">\n</form>##TableMarker##\n<iframe name=\"dummyframe\" id=\"dummyframe\" style=\"display: none;\">\n</iframe>\n</body>";  
                public static string collection = "<!DOCTYPE html>\n<link rel=\"icon\" href=\"data:,\">\n<meta charset=\"utf-8\">\n<form method=\"post\" style = \"float: right\" action=\"shutdown\"><input type=\"submit\" value=\"Shutdown\">\n</form>\n<h1>\n##TitleMarker##\n</h1>\n<body>##TableMarker##\n</body>";
                public static string shutdown = "<!DOCTYPE html>\n<h1>\nServer closed. You can now close this tab.\n</h1>";
            }
        }

        public static class Wildcards {
            public static string tableMarker = "##TableMarker##";
            public static string titleMarker = "##TitleMarker##";
        }    

        public static class Builders{
            public static string indexTableHeader = "\n<table style=\"float: left\">\n<tr>\n<th style=\"text-align: left\" colspan=\"2\">\nAll Bands\n</th>\n</tr>";
            public static string indexMainRow = "\n<tr>\n<td>\n<form target=\"dummyframe\" method=\"post\"><input type=\"image\" name=\"{bandName}\" onclick=\"this.classList.toggle('unfavourited')\" src=\"data: image / png; base64, {favIcon}\" width=\"15\" height=\"15\">\n</form>\n</td>\n<td>\n<a href = \"{bandNameUrl}\">\n{bandName}\n</a>\n</td>\n</tr>";
            public static string collectionMainRow = "\n<table style=\"float: left;\">\n<td>\n<a href = \"{itemUrl}\">\n<img src=\"data: image / png; base64, {itemImage} \" width=\"206\" height=\"300\" alt=\"{itemName}\">\n</a>\n</td>\n<tr>\n<th>\n{itemPrice}\n</th>\n</tr>\n</table>";
            public static string favouritesTableHeader = "\n</table>\n<table style=\"float: left\">\n<tr>\n<th style=\"text-align: left\">\nFavourites\n</th>\n</tr>\n<tr>\n<td>\n<form method=\"post\" action=\"favourites\">\n<input type=\"submit\" value=\"Show All\">\n</form>\n</td>\n</tr>";
            public static string favouritesRow = "\n<tr>\n<td>\n<a href = \"{bandNameUrl}\">\n{bandName}\n</a>\n</td>\n</tr>";

        }
    }
}