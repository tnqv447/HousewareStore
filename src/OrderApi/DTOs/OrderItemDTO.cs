using OrderApi.Models;

namespace OrderApi.DTOs
{
    public class OrderItemDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public int Units { get; set; }
        public string OwnerId { get; set; }
        public OrderItemStatus Status { get; set; }

        public int OrderId { get; set; }
    }
}