using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderApi.Infrastructure.Repositories;
using OrderApi.Models;

namespace OrderApi.Data.Repositories
{
    public class OrderItemRepository : EFRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(OrderContext context) : base(context)
        {
        }

        public bool ItemExists(int orderId, int itemId)
        {
            var item = (Database.Set<Order>().Find(orderId)).OrderItems.Where(m => m.ItemId.Equals(itemId)).ElementAt(0);
            return item != null;
        }
    }
}