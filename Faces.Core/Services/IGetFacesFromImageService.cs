namespace Faces.Core.Services
{
    public interface IGetFacesFromImageService
    {
        Task<List<byte[]>> Get(byte[] image);
    }
}