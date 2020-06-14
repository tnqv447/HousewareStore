using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;

namespace MvcClient.Services
{
    public interface ICartService
    {
        Task<Cart> GetCart(Buyer buyer);
        Task AddItemToCart(Buyer buyer, CartItem cartItem);
        Task RemoveItemCart(Buyer buyer, string id);
        Task<Cart> SetQuantities(Buyer user, Dictionary<string, int> quantities);
        Task<Cart> UpdateCart(Cart cart);
        Task CheckQuantitiesCart( Cart cart);
        Task ClearCart(Buyer user);
        Order MapCartToOrder(Cart cart);
    }
}