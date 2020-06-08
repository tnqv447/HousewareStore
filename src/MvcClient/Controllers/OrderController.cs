using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcClient.Models;
using MvcClient.Services;
using System;
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

            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var user = _identitySvc.Get(User);
            var cart = await _cartSvc.GetCart(user);
            var order = _cartSvc.MapCartToOrder(cart);
            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.Address = user.Address.ToString();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order frmOrder)
        {
            if (!ModelState.IsValid)
            {
                return View(frmOrder);
            }
            
            var user = _identitySvc.Get(User);
            var order = frmOrder;
            Console.WriteLine(frmOrder.OrderId);
            order.BuyerId = user.Id;
            Console.WriteLine(frmOrder.OrderItems.Count);
            Console.WriteLine(order.OrderId);
            // var chargeSvc = new ChargeService();
            // var charge = chargeSvc.Create(new ChargeCreateOptions
            // {
            //     Amount = (int)(order.Total * 100),
            //     Currency = "usd",
            //     Description = $"Order Payment {order.UserName}",
            //     ReceiptEmail = order.UserName,
            //     Source = order.StripeToken
            // });

            var succeeded = true;
            // if (charge.Status == "succeeded")
            if (succeeded)
            {
                int orderId = await _orderSvc.CreateOrder(order);
                Console.WriteLine(orderId);
                await _cartSvc.ClearCart(user);
                return RedirectToAction("Complete", new { id = orderId});
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