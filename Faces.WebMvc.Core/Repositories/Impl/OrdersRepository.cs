namespace Faces.WebMvc.Core.Repositories.Impl
{
    using Faces.WebMvc.Core.Models.Order;
    using Faces.WebMvc.Core.Repositories;
    using Faces.WebMvc.Core.RestClient;
    using Faces.WebMvc.Core.Utils;
    using Microsoft.Extensions.Logging;

    public class OrdersRepository : IOrdersRepository
    {
        private readonly ILogger<OrdersRepository> logger;
        private readonly IOrderManagementApi orderManagementApi;

        public OrdersRepository(ILogger<OrdersRepository> logger, IOrderManagementApi orderManagementApi)
        {
            this.logger = logger;
            this.orderManagementApi = orderManagementApi;
        }

        public async Task<List<Order>> GetOrders()
        {
            var orders = await this.orderManagementApi.GetOrders();
            if (orders == null ||
                !orders.Any())
            {
                return new List<Order>();
            }

            foreach (var order in orders)
            {
                order.ImageString = ImageUtility.ConvertAndFormatImageToString(order.ImageData);
            }

            return orders;
        }

        public async Task<Order> GetByOrderId(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                this.logger.LogError("Order id null or empty");
                throw new ArgumentNullException(nameof(orderId));
            }

            Guid.TryParse(orderId, out var orderIdParsed);

            var order = await this.orderManagementApi.GetOrderById(orderIdParsed);
            if (order == null)
            {
                var errorMessage = $"Order: {orderId} not found";
                this.logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            order.ImageString = ImageUtility.ConvertAndFormatImageToString(order.ImageData);

            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.ImageString = ImageUtility.ConvertAndFormatImageToString(orderDetail.FaceData);
            }

            return order;
        }
    }
}