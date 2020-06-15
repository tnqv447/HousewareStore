using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MvcClient.Infrastructure;
using MvcClient.Models;

namespace MvcClient.Services
{
    public class CartService : ICartService
    {
        private readonly string _serviceBaseUrl;
        private readonly IHttpClient _httpClient;
        

        public CartService(IHttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _serviceBaseUrl = $"{appSettings.Value.CartUrl}/api/cart";
        }

        public async Task<Cart> GetCart(Buyer buyer)
        {
            var uri = _serviceBaseUrl + $"/{buyer.Id}";

            var cart = await _httpClient.GetAsync<Cart>(uri);
            

            return cart ?? new Cart { Id = buyer.Id };
        }

        public async Task<Cart> UpdateCart(Cart cart)
        {
            var uri = _serviceBaseUrl;

            return await _httpClient.PutAsync<Cart>(uri, cart);
        }
        public async Task RemoveItemCart(Buyer user, string id){
            var cart = await GetCart(user);
            var itemFound = cart.CartItems.Find(x => x.Id == id);
            
            if(itemFound != null){
                cart.CartItems.Remove(itemFound);
            }
            await UpdateCart(cart);
        }
        public async Task CheckQuantitiesCart(Cart cart){
            var itemFound = cart.CartItems.FindAll(m => m.Quantity == 0);
            foreach(var item in itemFound){
                cart.CartItems.Remove(item);
            }
            await UpdateCart(cart);
        }
        public async Task AddItemToCart(Buyer user, CartItem item)
        {
            var cart = await GetCart(user);

            var itemFound = cart.CartItems.Find(x => x.ItemId == item.ItemId);

            if (itemFound == null)
            {
                
                cart.CartItems.Add(item);

            }
            else
            {
                itemFound.Quantity++;

            }

            await UpdateCart(cart);
        }

        public Order MapCartToOrder(Cart cart)
        {
            var order = new Order();

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ItemId = int.Parse(item.ItemId),
                    ItemName = item.ItemName,
                    UnitPrice = item.UnitPrice,
                    OwnerId = item.OwnerId,
                    PictureUrl = item.PictureUrl,
                    Units = item.Quantity
                });
                order.Total += item.Quantity * item.UnitPrice;
            }

            return order;
        }

        public async Task ClearCart(Buyer buyer)
        {
            var uri = _serviceBaseUrl + $"/{buyer.Id}";

            await _httpClient.DeleteAsync(uri);
        }

        public async Task<Cart> SetQuantities(Buyer user, Dictionary<string, int> quantities)
        {
            var basket = await GetCart(user);

            basket.CartItems.ForEach(x =>
            {
                if (quantities.TryGetValue(x.Id, out var quantity))
                {
                    x.Quantity = quantity;
                }
            });

            return basket;
        }
    }
}