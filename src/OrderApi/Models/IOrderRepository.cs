using System.Collections.Generic;
using System.Threading.Tasks;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Models
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByBuyerAsync(string buyerId);
    }
}