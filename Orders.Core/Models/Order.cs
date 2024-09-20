namespace Orders.Core.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public string UserEmail { get; set; }
        public byte[] ImageData { get; set; }
        public Status Status { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
