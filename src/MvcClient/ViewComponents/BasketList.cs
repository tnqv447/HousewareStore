using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.ViewComponents
{
    public class BasketList : ViewComponent
    {
        private readonly ICartService _cartSvc;

        public BasketList(ICartService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(Buyer user)
        {
            var cart = await _cartSvc.GetCart(user);

            return View(cart);
        }

    }
}