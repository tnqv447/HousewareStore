using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcClient.Models;
using MvcClient.Services;
using System;
using MvcClient.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MvcClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IIdentityService<Buyer> _identitySvc;
        private readonly IOrderService _orderSvc;
        private readonly ICartService _cartSvc;
        private readonly AppSettings _settings;

        public OrderController(IOrderService orderService, ICartService cartService,
            IIdentityService<Buyer> identityService, IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
            _identitySvc = identityService;
            _cartSvc = cartService;
            _orderSvc = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var user = _identitySvc.Get(User);
            var orders = await _orderSvc.GetOrders(user.Id);
            BuyerViewModel orvm = new BuyerViewModel();
            orvm.order = orders;
            orvm.buyer = user;
            return View(orvm);
        }
        // [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = _identitySvc.Get(User);
            var cart = await _cartSvc.GetCart(user);
            Console.WriteLine("\n"+cart.CartItems[0].ItemName);
            var order = _cartSvc.MapCartToOrder(cart);
            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.PhoneNumber = user.PhoneNumber;
            order.Email =   user.Email;
            order.Address = user.Address;

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order frmOrder, string stripeToken)
        {
            if (!ModelState.IsValid)
            {
                return View(frmOrder);
            }
            
            var user = _identitySvc.Get(User);
            var order = frmOrder;
            Console.WriteLine("\n" + order.OrderItems[0].ItemName);
            order.BuyerId = user.Id;
            
            
            var chargeSvc = new Stripe.ChargeService();
            var charge = chargeSvc.Create(new Stripe.ChargeCreateOptions
            {
                Amount = (int)(order.Total * 100),
                Currency = "usd",
                Description = $"Order Payment {order.UserName}",
                ReceiptEmail = order.Email,
                Source = stripeToken
            });
            
            // var succeeded = true;
            if (charge.Status == "succeeded")
            // if (succeeded)
            {   
                order.PaymentAuthCode = charge.Status;
                int orderId = await _orderSvc.CreateOrder(order);
                Console.WriteLine(orderId);
                await _cartSvc.ClearCart(user);
                return RedirectToAction("Complete", new { id = orderId });
            }

            ViewData["message"] = "Payment cannot be processed, try again";

            return View(frmOrder);
        }

        public IActionResult Complete(int id, string userName)
        {
            return View(id);
        }

        public async Task<ActionResult<Order>> Details(int orderId)
        {
            var order = await _orderSvc.GetOrder(orderId);

            return View(order);
        }
    }
}