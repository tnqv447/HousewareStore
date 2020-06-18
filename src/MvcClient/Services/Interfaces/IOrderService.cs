using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;

namespace MvcClient.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetOrders(string userId);
        Task<IEnumerable<OrderItemForSales>> GetOrderItemsForSales(string salesId, SearchTypeOrderItem searchType = SearchTypeOrderItem.ItemName, string searchString = null,
                                OrderItemStatus status = OrderItemStatus.Preparing, SortTypeOrderItem sortType = SortTypeOrderItem.OrderId, SortOrderOrderItem sortOrder = SortOrderOrderItem.Ascending);
        Task<int> CreateOrder(Order order);
        Task<Order> GetOrder(int id);
        Task<OrderItem> GetOrderItem(int id);

        Task UpdateOrderItem(int orderId, OrderItem item);
    }
}