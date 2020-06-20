using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvcClient.Authorization;
using MvcClient.Models;
using MvcClient.Services;
using MvcClient.ViewModels;
namespace MvcClient.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAnalysisService _analysisService;
        private readonly IIdentityService<Buyer> _identityService;
        private readonly IOrderService _orderService;
        public AnalysisController(ILogger<AdminController> logger, IAnalysisService analysisService,
                                        IIdentityService<Buyer> identityService,IOrderService orderService)
        {
            _logger = logger;
            _analysisService = analysisService;
            _orderService = orderService;
            _identityService = identityService;
        }
        [Authorize(Roles = "Administrators, Sales, Managers")]
        public async Task<IActionResult> IndexAsync()
        {
            AnalysisViewModel indexView = new AnalysisViewModel();
            indexView.SalesCount = await _analysisService.CountAllSales();
            indexView.SalesCount = indexView.SalesCount.OrderByDescending(m => m.TotalPrices).ToList();
            if(User.IsInRole("Sales")){
                string id = _identityService.Get(User).Id;
                indexView.BuyersCount = await _analysisService.CountItemsByBuyersAsync(id);
                indexView.BuyersCount = indexView.BuyersCount.OrderByDescending(m => m.TotalPrices);
            }
            
            return View(indexView);
        }
        [Authorize(Roles = "Administrators, Managers")]
        public async Task<IActionResult> Sale(string saleName, string id)
        {
            // Console.WriteLine("\n asdasda: "+sale.UserId);
            var lstOrder = await _orderService.GetOrderItemsForSales(id);
            ViewData["saleName"] = saleName;
            var v = await _analysisService.CountItemsBySalesAsync(id);
            return View(v);
        }
    }
}