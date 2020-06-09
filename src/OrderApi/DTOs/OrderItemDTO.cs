namespace OrderApi.DTOs
{
    public class OrderItemDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public int Units { get; set; }
    }
}