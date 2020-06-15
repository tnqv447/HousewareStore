using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrderApi.Models;
namespace OrderApi.DTOs
{
    public class OrderItemForSalesDTO : OrderItemDTO
    {
        //additional info for sales
        public string BuyerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime OrderDate { get; set; }
        public Address Address { get; set; }
        public string PaymentAuthCode { get; set; }
        public string Note { get; set; }
    }
}