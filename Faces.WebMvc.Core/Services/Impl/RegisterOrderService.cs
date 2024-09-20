namespace Faces.WebMvc.Core.Services.Impl
{
    using Faces.WebMvc.Core.Models.Order.Requests;
    using MassTransit;
    using Messaging.Core.Commands;
    using Messaging.Core.Constants;

    public class RegisterOrderService : IRegisterOrderService
    {
        private readonly IBusControl busControl;

        public RegisterOrderService(IBusControl busControl)
        {
            this.busControl = busControl;
        }

        public async Task<Guid> Register(Stream imageStream, RegisterOrderRequest registerOrderRequest)
        {
            ArgumentNullException.ThrowIfNull(imageStream);
            ArgumentNullException.ThrowIfNull(registerOrderRequest);

            var imageMemoryStream = new MemoryStream();
            await imageStream.CopyToAsync(imageMemoryStream);

            var orderId = Guid.NewGuid();

            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMqUri}{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");
            var endPoint = await this.busControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>(new
            {
                OrderId = orderId,
                PictureUrl = registerOrderRequest.PictureUrl,
                UserEmail = registerOrderRequest.UserEmail,
                ImageData = imageMemoryStream.ToArray()
            });

            return orderId;
        }
    }
}