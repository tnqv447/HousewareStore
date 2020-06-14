using System.Collections.Generic;
using System.Threading.Tasks;
using OrderApi.Infrastructure.Repositories;
using OrderApi.Models;

namespace OrderApi.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByBuyerAsync(string buyerId);
        Task<IEnumerable<OrderItem>> GetBySalesAsync(string salesId);
    }
}