using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
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
            public static string favourited = "iVBORw0KGgoAAAANSUhEUgAAABUAAAAVCAYAAACpF6WWAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAE/SURBVDhPrZPRagIxEEVntW4LtuBL34T+Qv//b9qXgkihBUFd1/TeZKJJTGIKe8CdJDO5mWRG+QcrtZNi1N5lpnZSqqJdpwNkaT6ctd87VEVNIDGMOmjgmksZl+UjfgdseLNr1X1Nb3o643NU20DuxPDdjsiylzlGvL5azTYk0uEkenwtSJER2c6T+6WH+BPsu+32Iv1CZPGAFb8xVyBmTHDAcMJ1BpHl00U8aBpfEAoisBmNDwsYvQVwwsyypSgal3ZEKkqcMD21Vld/KkhyLdUx8GuDUc5LsE5/TpCUtrliVaj5i6Ivz/j46zMXVtznhHXrL5ATNeYT/8oeIxXbfmO4dtaL0689ffPyqaibM4yi7AD06es7F6WzFvOfX1j6r0Q6N48MXPWBFoKEcTazJCbSyYqqJTm/pzVuCkT+AF6CVURB95a8AAAAAElFTkSuQmCC";
            public static string notFavourited = "iVBORw0KGgoAAAANSUhEUgAAABUAAAAVCAYAAACpF6WWAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAEsSURBVDhPrZJLjoMwDIYd3hKbOQniCNx/BfdAYsMCDa/p73FoSBNIJb5FTYj74cSmL/iR+Ci7xFsiiY9yKVVKyRPtbdty5NUNl9J9fzvWdZWne0KOz1XmeU6h1Qbd6bZtXCliCMelGZiV/L6qy6IoYqGOdV3L9sHJg8XpOHJEL1puYn9Ef4HvbZomStOUkiQ5Ou86spaikcuy0DzPVBSFlr9n5gWL4zj+qtM6XwuPHwMWo0pznHzoPFMIbCkIEvuEwDVSCol93/MfXeA99l1C4J1T3NUVV/teaVmW8vRfGTpuVm7u27ike9d1lGUZLyAbhoGqquKoxwn7MtMfF29LeY0G4HiImNOmafBaIWI9jqN9/JPH230gjQBmHldm5Zw8TqlE4NrXhOY9AdEfPWONFBD8XLAAAAAASUVORK5CYII=";
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
                public static string index = "<!DOCTYPE html><link rel=\"icon\" href=\"data:,\"><meta charset=\"utf-8\"><body><form style=\"float: right\" method=\"post\" action=\"shutdown\"><input type=\"submit\" value=\"Shutdown\"></form>##TableMarker##</body>";  
                public static string collection = "<!DOCTYPE html><link rel=\"icon\" href=\"data:,\"><meta charset=\"utf-8\"></style><form method=\"post\" style = \"float: right\" action=\"shutdown\"><input type=\"submit\" value=\"Shutdown\"></form><h1>##TitleMarker##</h1><body>##TableMarker##</body>";
                public static string shutdown = "<!DOCTYPE html><h1>Server closed. You can now close this tab.</h1>";
            }
        }

        public static class Wildcards {
            public static string tableMarker = "##TableMarker##";
            public static string titleMarker = "##TitleMarker##";
        }    

        public static class Builders{
            public static string indexTableHeader = "<table style=\"float: left\"><tr><th style=\"text-align: left\" colspan=\"2\">All Bands</th></tr>";
            public static string indexMainRow = "<tr><td><a href = \"" + InternalUrls.favouritesSplitter + "{bandNameUrl}\"><img src=\"data: image / png; base64, {favIcon} \" width=\"15\" height=\"15\"></a></td><td><a href = \"{bandNameUrl}\">{bandName}</a></td></tr>";
            public static string collectionMainRow = "<table style=\"float: left;\"><td><a href = \"{itemUrl}\"><img src=\"data: image / png; base64, {itemImage} \" width=\"206\" height=\"300\" alt=\"{itemName}\"></a></td><tr><th>{itemPrice}</th></tr></table>";
            public static string favouritesTableHeader = "</table><table style=\"float: left\"><tr><th style=\"text-align: left\">Favourites</th></tr><tr><td><form method=\"post\" action=\"favourites\"><input type=\"submit\" value=\"Show All\"></form></td></tr>";
            public static string favouritesRow = "<tr><td><a href = \"{bandNameUrl}\">{bandName}</a></td></tr>";

        }
    }
}