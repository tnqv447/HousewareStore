namespace MvcClient.Models
{
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public string OwnerId { get; set; }
        public OrderItemStatus Status { get; set; }
        public int Units { get; set; }
    }
    public enum OrderItemStatus
    {
        Preparing = 0,
        Shipped = 1,
        Delivered = 2
    }
}