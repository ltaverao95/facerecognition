namespace Orders.Core.Messages.Consumers
{
    using MassTransit;
    using Messaging.Core.Commands;
    using Messaging.Core.Events;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Orders.Core.Hubs;
    using Orders.Core.Models;
    using Orders.Core.Models.Configuration;
    using Orders.Core.Models.Constants;
    using Orders.Core.Persistence.Repository;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<RegisterOrderCommandConsumer> logger;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly IOptions<OrderSettingsConfiguration> orderSettingsConfiguration;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository,
                                            IHttpClientFactory httpClientFactory,
                                            ILogger<RegisterOrderCommandConsumer> logger,
                                            IHubContext<OrderHub> hubContext,
                                            IOptions<OrderSettingsConfiguration> orderSettingsConfiguration)
        {
            this.orderRepository = orderRepository;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.hubContext = hubContext;
            this.orderSettingsConfiguration = orderSettingsConfiguration;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            try
            {
                var result = context.Message;

                if (string.IsNullOrEmpty(result.UserEmail) ||
                    string.IsNullOrEmpty(result.PictureUrl) ||
                    result.ImageData == null)
                {
                    this.logger.LogError("Error empty values to get faces");
                    throw new ArgumentException("Empty Values");
                }

                var orderRegistered = this.SaveOrder(result);

                await this.hubContext.Clients.All.SendAsync(SignalRHubConstants.UpdateOrdersCommand, "New Order Created", orderRegistered.OrderId);

                var httpClient = this.httpClientFactory.CreateClient();
                var orderDetailData = await GetFacesFromFaceApi(httpClient, orderRegistered);
                var faces = orderDetailData.Item1;
                var orderId = orderDetailData.Item2;

                this.SaveOrderDetails(orderId, faces);

                await this.hubContext.Clients.All.SendAsync(SignalRHubConstants.UpdateOrdersCommand, "Order processed", orderRegistered.OrderId);

                await context.Publish<IOrderProcessedEvent>(new 
                { 
                    OrderId = orderId,
                    PictureUrl = result.PictureUrl,
                    Faces = faces,
                    UserEmail = result.UserEmail,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error");
                throw;
            }
        }

        private void SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var order = this.orderRepository.GetOrderByIdAsync(orderId).Result;
            if (order == null)
            {
                return;
            }

            order.Status = Status.Processed;
            foreach (var face in faces)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = orderId,
                    FaceData = face
                };

                order.OrderDetails.Add(orderDetail);
            }

            this.orderRepository.UpdateOrder(order);
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApi(HttpClient client, Order orderRegistered)
        {
            var byteContent = new ByteArrayContent(orderRegistered.ImageData);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using var response =
                await client.PostAsync($"{orderSettingsConfiguration.Value.FacesApiUrl}/api/faces?orderId={orderRegistered.OrderId}", byteContent);
            var apiResponse = await response.Content.ReadAsStringAsync();
            var orderDetailData = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);

            return orderDetailData;
        }

        private Order SaveOrder(IRegisterOrderCommand result)
        {
            var order = new Order
            {
                OrderId = result.OrderId,
                UserEmail = result.UserEmail,
                Status = Status.Registered,
                PictureUrl = result.PictureUrl,
                ImageData = result.ImageData
            };

            var orderRegistered = this.orderRepository.RegisterOrder(order);

            return orderRegistered;
        }
    }
}