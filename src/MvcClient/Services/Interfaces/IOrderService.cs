using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;

namespace MvcClient.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetOrders(string userId);
        Task<int> CreateOrder(Order order);
        Task<Order> GetOrder(int id);
    }
}