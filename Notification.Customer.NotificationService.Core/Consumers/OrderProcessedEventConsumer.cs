namespace Notification.Customer.NotificationService.Core.Consumers
{
    using MassTransit;
    using Messaging.Core.Events;
    using Microsoft.Extensions.Logging;
    using Notification.Customer.EmailService.Models;
    using Notification.Customer.EmailService.Senders;
    using SixLabors.ImageSharp;
    using System.Threading.Tasks;

    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private readonly ILogger<OrderProcessedEventConsumer> logger;
        private readonly IEmailSender emailSender;

        public OrderProcessedEventConsumer(ILogger<OrderProcessedEventConsumer> logger, IEmailSender emailSender)
        {
            this.logger = logger;
            this.emailSender = emailSender;
        }

        public Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            try
            {
                this.logger.LogInformation($"{nameof(IOrderProcessedEvent)} event received");

                var orderProcessedEvent = context.Message;

                this.StoreFaces(orderProcessedEvent);

                var mailAddess = new string[] { orderProcessedEvent.UserEmail };

                this.emailSender.SendEmailAsync(new Message(mailAddess, $"Your order {orderProcessedEvent.OrderId}", "From FacesAndFaces", orderProcessedEvent.Faces));

                context.Publish<IOrderDispatchedEvent>(new
                {
                    OrderId = orderProcessedEvent.OrderId,
                    DispatchDateTime = DateTime.UtcNow
                });

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error in {nameof(OrderProcessedEventConsumer)}");
                throw;
            }
        }

        private void StoreFaces(IOrderProcessedEvent orderProcessedEvent)
        {
            if (orderProcessedEvent.Faces == null ||
                !orderProcessedEvent.Faces.Any())
            {
                this.logger.LogWarning("No faces detected");
                return;
            }

            var facesRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Faces");
            Directory.CreateDirectory($"{facesRootDirectory}/{orderProcessedEvent.OrderId}");

            for (var i = 0; i < orderProcessedEvent.Faces.Count; i++)
            {
                var face = orderProcessedEvent.Faces[i];
                var ms = new MemoryStream(face);
                var image = Image.Load(ms);
                var imageNamePath = $"{facesRootDirectory}/{orderProcessedEvent.OrderId}/face{i}.jpg";
                image.Save(imageNamePath);
                this.logger.LogInformation($"{imageNamePath} stored");
            }
        }
    }
}