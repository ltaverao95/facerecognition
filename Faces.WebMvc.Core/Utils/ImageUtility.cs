namespace Faces.WebMvc.Core.Utils
{
    public static class ImageUtility
    {
        public static string ConvertAndFormatImageToString(byte[] imageData)
        {
            var imageBase64Data = Convert.ToBase64String(imageData);
            return string.Format("data:image/png;base64, {0}", imageBase64Data);
        }
    }
}