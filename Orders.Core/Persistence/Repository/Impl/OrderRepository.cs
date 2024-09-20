namespace Orders.Core.Persistence.Repository.Impl
{
    using Microsoft.EntityFrameworkCore;
    using Orders.Core.Models;

    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext ordersContext;

        public OrderRepository(OrdersContext ordersContext)
        {
            this.ordersContext = ordersContext;
        }

        public Order GetOrderById(Guid id)
        {
            return this.ordersContext.Orders
                .Include(x => x.OrderDetails)
                .FirstOrDefault(x => x.OrderId == id);
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await this.ordersContext.Orders
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await this.ordersContext.Orders.ToListAsync();
        }

        public Order RegisterOrder(Order order)
        {
            if (this.ordersContext.Orders.Contains(order))
            {
                return order;
            }

            var registeredOrder = this.ordersContext.Add(order);
            this.ordersContext.SaveChanges();
            return registeredOrder.Entity;
        }

        public void UpdateOrder(Order order)
        {
            this.ordersContext.Entry(order).State = EntityState.Modified;
            this.ordersContext.SaveChanges();
        }
    }
}