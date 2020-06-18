namespace MvcClient.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public string OwnerId { get; set; }
        public OrderItemStatus Status { get; set; }
        public int Units { get; set; }
        public int OrderId { get; set; }
    }
    public enum OrderItemStatus
    {
        Preparing = 0,
        Shipping = 1,
        Delivered = 2,
        Accepted = 3,
        Rejected = 4,
        AllStatus = 5
    }
    public enum SortOrderOrderItem
    {
        Descending = 0,
        Ascending = 1,
    }
    public enum SortTypeOrderItem
    {
        OrderId = 0,
        BuyerName = 1,
        ItemName = 2,
        Status = 3,
    }
    public enum SearchTypeOrderItem
    {
        BuyerName = 0,
        ItemName = 1
    }
}