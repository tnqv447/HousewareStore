using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;
using MvcClient.ViewModels;
using Polly.CircuitBreaker;

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
                vm.ItemsInCart = cart.CartItems.Count();
                vm.TotalCost = cart.Total();
            }
            catch (BrokenCircuitException)
            {
                ViewBag.IsCartInoperative = true;
            }

            return View(vm);
        }
    }
}