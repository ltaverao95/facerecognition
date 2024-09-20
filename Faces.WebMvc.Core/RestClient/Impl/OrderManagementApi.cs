namespace Faces.WebMvc.Core.RestClient.Impl
{
    using Faces.WebMvc.Core.Models;
    using Faces.WebMvc.Core.Models.Order;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Refit;
    using System.Net;

    public class OrderManagementApi : IOrderManagementApi
    {
        private readonly IOrderManagementApi restClient;
        private readonly ILogger<OrderManagementApi> logger;
        private readonly IOptions<FacesWebConfiguration> facesWebConfiguration;

        public OrderManagementApi(ILogger<OrderManagementApi> logger,
                                  HttpClient httpClient,
                                  IOptions<FacesWebConfiguration> facesWebConfiguration)
        {
            this.facesWebConfiguration = facesWebConfiguration;
            httpClient.BaseAddress = new Uri($"{this.facesWebConfiguration.Value.OrdersApiUrl}/api");

            this.restClient = RestService.For<IOrderManagementApi>(httpClient);
            this.logger = logger;
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            try
            {
                return await this.restClient.GetOrderById(orderId);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                this.logger.LogError(ex, "Error getting order by id");

                throw;
            }
        }

        public async Task<List<Order>> GetOrders()
        {
            return await this.restClient.GetOrders();
        }
    }
}