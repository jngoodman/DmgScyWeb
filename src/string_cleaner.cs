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