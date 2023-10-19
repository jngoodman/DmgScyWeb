using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

namespace DmgScy;

public class ImageDownloader{
    public string source;
    public string imagePath{get; set;}
    
    public ImageDownloader(string url, string imageName){
        this.imagePath = GetPath(imageName);       
        this.source = url.Split(Constants.imageRightPartEncoder)[0];
        if(!source.StartsWith("https")){
            source = "https:" + source;
        }        
        Download().Wait();
    }

    private string GetPath(string imageName){
        Directory.CreateDirectory(Constants.imageDir);
        string cleanName = StringCleaner.EraseIllegalChars(imageName);
        string fileName = $"{cleanName}{Constants.imageExtension}";
        return Constants.imageDir + $"/{fileName}";   
    }

    private async Task Download(){ 
        HttpClient client = new HttpClient();
        byte[] imageBytes = await client.GetByteArrayAsync(source);
        await File.WriteAllBytesAsync(imagePath, imageBytes);
    }
}