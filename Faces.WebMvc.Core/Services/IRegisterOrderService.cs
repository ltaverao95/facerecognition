namespace Faces.WebMvc.Core.Services
{
    using Faces.WebMvc.Core.Models.Order.Requests;

    public interface IRegisterOrderService
    {
        Task<Guid> Register(Stream image, RegisterOrderRequest registerOrderRequest);
    }
}