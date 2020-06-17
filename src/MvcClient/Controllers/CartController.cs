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
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action, string Id)
        {

            if (action == "[ Checkout ]")
            {

                return RedirectToAction("Create", "Order");
                // return RedirectToAction("Index","Cart");
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
        [HttpGet]
        public async Task<IActionResult> Cart(){
            var buyer = _identitySvc.Get(User);
            var cart = await _cartSvc.GetCart(buyer);
            return new JsonResult(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AllAction(Dictionary<string, int> quantities,string action,string id){
            string msg="Fail";
            if(action == "[ Clear Item ]"){
                var buyer = _identitySvc.Get(User);
                await _cartSvc.RemoveItemCart(buyer, id);
                msg = "Succesfull";
                
            }
            else if(action=="[ Update ]"){
                var buyer = _identitySvc.Get(User);
                Cart upCart = await _cartSvc.GetCart(buyer);
                foreach(var item in upCart.CartItems){
                    if(quantities[item.Id] != item.Quantity)
                    {
                        item.Quantity = quantities[item.Id];
                    }
                }
                await _cartSvc.UpdateCart(upCart);
                await _cartSvc.CheckQuantitiesCart(upCart);
                msg="Succesfull";
            }
            else if(action=="[ Clear ]"){
                var buyer = _identitySvc.Get(User);
                await _cartSvc.ClearCart(buyer);
                msg="Succesfull";
            }
            return new JsonResult(msg);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(Item item, int quantity)
        {
            var buyer = _identitySvc.Get(User);
            
            if(quantity != 0){
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ItemId = item.Id.ToString(),
                    ItemName = item.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = quantity,
                    OwnerId = item.OwnerId,
                    PictureUrl = item.PictureUrl
                };
                await _cartSvc.AddItemToCart(buyer, cartItem);
            }
            

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