namespace Faces.WebMvc.Core.RestClient
{
    using Faces.WebMvc.Core.Models.Order;
    using Refit;

    public interface IOrderManagementApi
    {
        [Get("/orders")]
        Task<List<Order>> GetOrders();

        [Get("/orders/{orderId}")]
        Task<Order> GetOrderById(Guid orderId);
    }
}