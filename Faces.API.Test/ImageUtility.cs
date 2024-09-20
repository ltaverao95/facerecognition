namespace Faces.API.Test
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageUtility
    {
        public byte[] ConvertToBytes(string imgPath)
        {
            var memoryStream = new MemoryStream();
            using var stream = new FileStream(imgPath, FileMode.Open);
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            return bytes;
        }

        public void FromBytesToImage(byte[] imageBytes, string fileName)
        {
            using var ms = new MemoryStream(imageBytes);
            Image img = Image.FromStream(ms);
            img.Save($"{fileName}.jpg", ImageFormat.Jpeg);
        }
    }
}
