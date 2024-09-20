namespace Faces.WebMvc.Core.Repositories
{
    using Faces.WebMvc.Core.Models.Order;

    public interface IOrdersRepository
    {
        Task<List<Order>> GetOrders();

        Task<Order> GetByOrderId(string orderId);
    }
}