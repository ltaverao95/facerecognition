namespace Faces.WebMvc.Core.Models.Order
{
    using Faces.WebMvc.Core.Models.Order.Enums;
    using System.Text.Json.Serialization;

    public class Order
    {
        public Guid OrderId { get; set; }
        public string UserEmail { get; set; }
        public string PictureUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageString { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}