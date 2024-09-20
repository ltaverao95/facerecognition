namespace Faces.API.Controllers
{
    using Faces.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class FacesController : ControllerBase
    {
        private readonly ILogger<FacesController> _logger;
        private readonly IGetFacesFromImageService getFacesFromImageService;

        public FacesController(ILogger<FacesController> logger, IGetFacesFromImageService getFacesFromImageService)
        {
            _logger = logger;
            this.getFacesFromImageService = getFacesFromImageService;
        }

        [HttpPost]
        public async Task<Tuple<List<byte[]>, Guid>> ReadFaces(Guid orderId)
        {
            try
            {
                using var ms = new MemoryStream(2048);
                await Request.Body.CopyToAsync(ms);
                var faces = await this.getFacesFromImageService.Get(ms.ToArray());
                return new Tuple<List<byte[]>, Guid>(faces, orderId);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error generating faces");
                throw;
            }

        }
    }
}