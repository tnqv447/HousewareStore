using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MvcClient.Infrastructure;
using MvcClient.Models;

namespace MvcClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly string _serviceBaseUrl;
        private readonly IHttpClient _httpClient;

        public OrderService(IHttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _serviceBaseUrl = $"{appSettings.Value.OrderUrl}/api/orders";
        }

        public async Task<int> CreateOrder(Order order)
        {
            var uri = _serviceBaseUrl;

            var orderOut = await _httpClient.PostAsync<Order>(uri, order);

            return orderOut.OrderId;
        }

        public async Task<Order> GetOrder(int id)
        {
            var uri = _serviceBaseUrl + $"/{id}";

            return await _httpClient.GetAsync<Order>(uri);
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            var uri = _serviceBaseUrl;

            return await _httpClient.GetListAsync<Order>(uri);
        }

        public async Task<IEnumerable<Order>> GetOrders(string userId)
        {
            var uri = _serviceBaseUrl + $"/buyerid/{userId}"; ;

            return await _httpClient.GetListAsync<Order>(uri);
        }
    }
}