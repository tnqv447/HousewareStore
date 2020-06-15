using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace MvcClient.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$", ErrorMessage = "Your first name is invalid")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$", ErrorMessage = "Your last name is invalid")]
        public string LastName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public Address Address { get; set; }
        public string PaymentAuthCode { get; set; }
        public double Total { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Your phone is invalid")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your email is invalid")]
        public string Email { get; set; }

        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardExpirationShort { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardType { get; set; }
        public string StripeToken { get; set; }
        public string Note { get; set; }
        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Preparing = 0,
        Shipping = 1,
        Delivered = 2,
        Accepted = 3,
        Rejected = 4
    }
}