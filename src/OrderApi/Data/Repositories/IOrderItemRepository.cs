using OrderApi.Infrastructure.Repositories;
using OrderApi.Models;

namespace OrderApi.Data.Repositories
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        bool ItemExists(int orderId, int ItemId);

    }
}