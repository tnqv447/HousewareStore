using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderApi.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public virtual Address Address { get; set; }
        public string PaymentAuthCode { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
        public virtual IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem> ();
    }

    public enum OrderStatus {
        Preparing = 0,
        Shipped = 1,
        Delivered = 2
    }

    [Owned]
    public class Address
    {
        //[JsonProperty("street_address")]
        [Display (Name = "Street address")]
        public string StreetAddress { get; set; }

        [Display (Name = "Locality/District")]
        public string Locality { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        //[JsonProperty("postal_code")]
        [Display (Name = "Postal code")]
        public string PostalCode { get; set; }
        public Address () {

        }
        public Address (string StreetAddress, string Locality, string City, string Country, string PostalCode) {
            this.StreetAddress = StreetAddress;
            this.Locality = Locality;
            this.City = City;
            this.Country = Country;
            this.PostalCode = PostalCode;
        }
        public override string ToString () {
            return $"{StreetAddress}, {Locality}, {City}, {Country}, {PostalCode}";
        }
    }
}