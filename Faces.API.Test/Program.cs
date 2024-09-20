// See https://aka.ms/new-console-template for more information
using Faces.API.Test;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var imagePath = @"oscars-1.jpg";
        var guid = Guid.NewGuid();
        var urlAddress = $"http://localhost:44353/api/faces?orderId={guid}";
        
        try
        {
            var imageutility = new ImageUtility();
            var bytes = imageutility.ConvertToBytes(imagePath);

            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            using var httpClient = new HttpClient();
            using var response = await httpClient.PostAsync(urlAddress, byteContent);
            var apiResponse = await response.Content.ReadAsStringAsync();
            var faceTouple = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);

            if (faceTouple == null ||
                !faceTouple.Item1.Any())
            {
                return;
            }

            var facesList = faceTouple.Item1;

            for (var i = 0; i < facesList.Count; i++)
            {
                imageutility.FromBytesToImage(facesList[i], $"face{i}");
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        Console.WriteLine("End");
        Console.ReadLine();
    }
}