namespace Faces.Core.Services.Impl
{
    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    public class GetFacesFromImageService : IGetFacesFromImageService
    {
        private readonly ILogger<GetFacesFromImageService> logger;
        private readonly IConfiguration configuration;

        public GetFacesFromImageService(ILogger<GetFacesFromImageService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<List<byte[]>> Get(byte[] imageBytes)
        {
            ArgumentNullException.ThrowIfNull(imageBytes);

            var imgGuid = Guid.NewGuid();
            var facesRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Faces");
            Directory.CreateDirectory($"{facesRootDirectory}/{imgGuid}");

            var img = Image.Load(imageBytes);
            img.Save($"{facesRootDirectory}/{imgGuid}/original.jpg");

            var faceList = await GetFacesFromAzureService(imageBytes, imgGuid, facesRootDirectory, img);

            return faceList;
        }

        private async Task<List<byte[]>> GetFacesFromAzureService(byte[] imageBytes, Guid imgGuid, string facesRootDirectory, Image img)
        {
            string subKey = this.configuration["AzureSubscriptionKey"]!;
            string endPoint = this.configuration["AzureEndPoint"]!;

            try
            {
                var client = Authenticate(endPoint, subKey);
                var faceList = new List<byte[]>();

                var faces = await client.Face.DetectWithStreamAsync(new MemoryStream(imageBytes), false, false, null);

                for (var i = 0; i < faces.Count; i++)
                {
                    var face = faces[i];
                    var s = new MemoryStream();
                    var zoom = 1.0;
                    int h = (int)(face.FaceRectangle.Height / zoom);
                    int w = (int)(face.FaceRectangle.Width / zoom);
                    int x = face.FaceRectangle.Left;
                    int y = face.FaceRectangle.Top;

                    var imageCloned = img.Clone(ctx => ctx.Crop(new Rectangle(x, y, w, h)));
                    imageCloned.Save($"{facesRootDirectory}/{imgGuid}/face" + i + ".jpg");
                    imageCloned.SaveAsJpeg(s);
                    faceList.Add(s.ToArray());
                }

                return faceList;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Couldn't detect faces");
                throw;
            }
        }

        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}