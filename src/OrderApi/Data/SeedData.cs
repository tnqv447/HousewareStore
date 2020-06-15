using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderApi.Models;

namespace OrderApi.Data
{
    public class SeedData
    {
        public static async Task Initialize(OrderContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!context.Orders.Any())
            {
                await context.AddAsync(new Order
                {
                    FirstName = "Alice",
                    LastName = "Smith",
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Preparing,
                    Address = new Address
                    {
                        StreetAddress = "One Hacker Way",
                        Locality = "Heidelberg",
                        City = "Heidelberg",
                        Country = "Germany",
                        PostalCode = "69118"
                    },
                    PaymentAuthCode = "Code",
                    Total = 99.9M,
                    Note = "a",
                    OrderItems = new List<OrderItem>{
                        new OrderItem(1, "Instant Pot Duo 7-in-1 Electric Pressure Cooker",79M, "item_4.jpg","6cbb393b-2043-4330-998f-032d5f0ea957", OrderItemStatus.Preparing)
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}