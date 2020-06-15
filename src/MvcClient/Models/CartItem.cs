namespace MvcClient.Models
{
    public class CartItem
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public double OldUnitPrice { get; set; }
        public string OwnerId { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
    }
}