using System.Drawing;

namespace DmgScy;

public static class JpegConverter{
    public static string ToBase64(Image image){
        MemoryStream stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        byte[] imageBytes = stream.ToArray();
        string imageBase64 = Convert.ToBase64String(imageBytes);
        return imageBase64;
    }
}