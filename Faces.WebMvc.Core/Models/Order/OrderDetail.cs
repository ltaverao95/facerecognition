namespace Faces.WebMvc.Core.Models.Order
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public byte[] FaceData { get; set; }
        public string ImageString { get; set; }
    }
}