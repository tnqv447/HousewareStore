using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Services;
using Polly.CircuitBreaker;

namespace MvcClient.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartSvc;
        private readonly IIdentityService<Buyer> _identitySvc;

        public CartController(ICartService cartSvc, IIdentityService<Buyer> identitySvc)
        {
            _cartSvc = cartSvc;
            _identitySvc = identitySvc;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {

            if (action == "[ Checkout ]")
            {

                return RedirectToAction("Create", "Order");
                // return RedirectToAction("Index","Cart");
            }
            else if (action == "[ Clear ]")
            {
                var buyer = _identitySvc.Get(User);

                await _cartSvc.ClearCart(buyer);

                return RedirectToAction("Index", "Cart");
            }
            else if (action == "[ Update ]")
            {
                Cart upCart = new Cart();
                await _cartSvc.UpdateCart(upCart);
                return RedirectToAction("Index", "Cart");
            }
            try
            {
                var user = _identitySvc.Get(HttpContext.User);
                var cart = await _cartSvc.SetQuantities(user, quantities);
                var vm = await _cartSvc.UpdateCart(cart);
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Item item)
        {


            var cartItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = item.Id.ToString(),
                ItemName = item.Name,
                UnitPrice = item.UnitPrice,
                Quantity = 1,
                PictureUrl = item.PictureUrl
            };



            var buyer = _identitySvc.Get(User);

            await _cartSvc.AddItemToCart(buyer, cartItem);

            var listCart = await _cartSvc.GetCart(buyer);

            return new JsonResult(listCart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {

            var buyer = _identitySvc.Get(User);

            await _cartSvc.ClearCart(buyer);

            return RedirectToAction("Index", "Cart");
        }

        private void HandleBrokenCircuitException()
        {
            TempData["BasketInoperativeMsg"] = "cart Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }
    }
}