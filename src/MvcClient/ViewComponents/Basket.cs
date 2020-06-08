using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;
using MvcClient.ViewModels;
using Polly.CircuitBreaker;
using System.Collections.Generic;

namespace MvcClient.ViewComponents
{
    public class Basket : ViewComponent
    {
        private readonly ICartService _cartSvc;

        public Basket(ICartService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(Buyer user)
        {
            var vm = new CartComponentViewModel();

            try
            {
                var cart = await _cartSvc.GetCart(user);
                vm.ItemsInCart = this.CartItemsCount(cart.CartItems);
                vm.TotalCost = cart.Total();
            }
            catch (BrokenCircuitException)
            {
                ViewBag.IsCartInoperative = true;
            }

            return View(vm);
        }

        private int CartItemsCount(List<CartItem> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                count += item.Quantity;
            }
            return count;
        }
    }
}