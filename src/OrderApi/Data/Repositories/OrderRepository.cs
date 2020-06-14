using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderApi.Infrastructure.Repositories;
using OrderApi.Models;

namespace OrderApi.Data.Repositories
{
    public class OrderRepository : EFRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetByBuyerAsync(string buyerId)
        {
            return await Context.Orders
                .Where(o => o.BuyerId == buyerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetBySalesAsync(string salesId)
        {
            return await Context.OrderItems
                .Where(o => o.OwnerId.Equals(salesId))
                .ToListAsync();
        }

        private OrderContext Context => Database as OrderContext;
    }
}