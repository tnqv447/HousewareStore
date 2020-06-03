namespace MvcClient.Models
{
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public string PictureUri { get; set; }
        public int Units { get; set; }
    }
}