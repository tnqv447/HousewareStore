using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcClient.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal Total()
        {
            return Math.Round(CartItems.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}