using OrderApi.Infrastructure.Exceptions;

namespace OrderApi.Models
{
    public class OrderItem
    {
        public OrderItem()
        {
        }
        public OrderItem(int itemId, string itemName, decimal unitPrice, string pictureUrl, string ownerId, OrderItemStatus status, int units = 1)
        {
            if (units <= 0)
            {
                throw new OrderingDomainException("Invalid number of units");
            }
            ItemId = itemId;
            ItemName = itemName;
            UnitPrice = unitPrice;
            PictureUrl = pictureUrl;
            Units = units;
            OwnerId = ownerId;
            Status = status;
        }

        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public int Units { get; set; }
        public string OwnerId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public OrderItemStatus Status { get; set; }
        public void SetPictureUri(string pictureUrl)
        {
            PictureUrl = pictureUrl;
        }
        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new OrderingDomainException("Invalid units");
            }

            Units += units;
        }


    }

    public enum OrderItemStatus
    {
        Preparing = 0,
        Shipping = 1,
        Delivered = 2,
        Accepted = 3,
        Rejected = 4
    }
}