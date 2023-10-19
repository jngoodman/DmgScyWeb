using System.Drawing;

namespace DmgScy;

public static class StringCleaner{

    public static string EraseIllegalChars(string inputString){
        inputString = inputString.ToLower();
        List<string> outputList = new List<string>();
        foreach(char character in inputString){
            if(Constants.legalTableCharacters.Contains(character)){
                outputList.Add(character.ToString());
            }
        }
        string outputString = string.Join("", outputList);
        return outputString;        
    }
}

public static class JpegConverter{
    public static string ToBase64(Image image){
        MemoryStream stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        byte[] imageBytes = stream.ToArray();
        string imageBase64 = Convert.ToBase64String(imageBytes);
        return imageBase64;
    }
}

public class ImageDownloader{
    public string source;
    public string imagePath{get; set;}
    
    public ImageDownloader(string url, string imageName){
        this.imagePath = GetPath(imageName);       
        this.source = url.Split(Constants.imageRightPartEncoder)[0];
        if(!source.StartsWith("https")){
            source = "https:" + source;
        }        
        try{
            Download().Wait();
        }
        catch{
            //Sometimes, Task.Wait() throws an error, but if you just wait long enough it works!
            Console.WriteLine("Image downloading error.");
        }
    }

    private string GetPath(string imageName){
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