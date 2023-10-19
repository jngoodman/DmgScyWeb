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

public class UrlToImage{
    public string source;
    public string image {get; set;}
    
    public UrlToImage(string url){     
        this.source = url.Split(Constants.imageRightPartEncoder)[0];
        if(!source.StartsWith("https")){
            source = "https:" + source;
        }
        Task<byte[]> getBytes = GetByteArray();
        getBytes.Wait();
        byte[] imageBytes = getBytes.Result;
        this.image = Convert.ToBase64String(imageBytes);
    }

    private async Task<byte[]> GetByteArray(){ 
        HttpClient client = new HttpClient();
        return await client.GetByteArrayAsync(source);
    }
}