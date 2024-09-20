namespace Orders.Core.Persistence.Repository
{
    using Orders.Core.Models;

    public interface IOrderRepository
    {
        Order GetOrderById(Guid id);
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Order RegisterOrder(Order order);
        void UpdateOrder(Order order);
    }
}
