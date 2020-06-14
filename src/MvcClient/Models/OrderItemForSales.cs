using System;

namespace MvcClient.Models
{
    public class OrderItemForSales : OrderItem
    {
        //additional info for sales
        public string BuyerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string PaymentAuthCode { get; set; }
    }
}