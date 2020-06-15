using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcClient.Models;
using MvcClient.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using MvcClient.Authorization;
using MvcClient.ViewModels;
namespace MvcClient.Controllers
{
    [Authorize(Roles = "Sales")]
    public class OrderForSaleController : Controller
    {
        private readonly ILogger<OrderForSaleController> _logger;
        private readonly IOrderService _orderService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;
        public OrderForSaleController(ILogger<OrderForSaleController> logger, IOrderService orderService,
                             IAuthorizationService authorizationService, IIdentityService<Buyer> identityService)
        {
            _logger = logger;
            _orderService = orderService;
            _authorizationService = authorizationService;
            _identityService = identityService;
        }
        public async Task<IActionResult> Index(string searchItemName, int pageNumber = 1)
        {
            OrderForSaleViewModel viewModel = new OrderForSaleViewModel();
            var pageSize = 5;
            var saleId = User.IsInRole(Constants.SalesRole) ? _identityService.Get(User).Id : null;
            viewModel.OrderItems = await _orderService.GetOrderItemsForSales(saleId);

            viewModel.OrderItemsPaging = PaginatedList<OrderItemForSales>.Create(viewModel.OrderItems, pageNumber, pageSize);
            viewModel.PageIndex = pageNumber;
            viewModel.PageTotal = viewModel.OrderItemsPaging.TotalPages;
            return View(viewModel);
        }

    }
}