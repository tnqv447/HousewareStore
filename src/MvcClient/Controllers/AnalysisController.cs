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
        private readonly IUserService _userService;
        public AnalysisController(ILogger<AdminController> logger, IAnalysisService analysisService,
                                        IIdentityService<Buyer> identityService,IOrderService orderService,
                                        IUserService userService)
        {
            _logger = logger;
            _analysisService = analysisService;
            _orderService = orderService;
            _identityService = identityService;
            _userService = userService;
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
                indexView.AllItems = await _analysisService.CountAllProducts(id);
                indexView.AllItems = indexView.AllItems.OrderByDescending(m => m.TotalPrices);
            }
            if(User.IsInRole("Administrators")){
                indexView.AllBuyers = await _analysisService.CountItemAllBuyers();
                indexView.AllBuyers = indexView.AllBuyers.OrderByDescending(m => m.TotalPrices).ToList();
                indexView.AllItems = await _analysisService.CountAllProducts();
                indexView.AllItems = indexView.AllItems.OrderByDescending(m => m.TotalPrices);
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
        [Authorize(Roles = "Administrators, Sales")]
        public async Task<IActionResult> Buyer(string id, string buyerName){
            if(User.IsInRole("Sales")){
                string saleId = _identityService.Get(User).Id;
                var listBuyer = await _userService.GetBuyers();
                var buyer = listBuyer.Where(m => m.UserId.Equals(id)).FirstOrDefault();
                var v = await _analysisService.CountItemInBuyer(id,saleId);
                v.User = buyer;
                v.Count = v.Count.OrderByDescending(m => m.TotalPrices);
                return View(v);
            }
            if(User.IsInRole("Administrators")){
                var listBuyers = await _analysisService.CountItemAllBuyers();
                var v = listBuyers.Where(m => m.User.UserId.Equals(id)).FirstOrDefault();
                v.Count = v.Count.OrderByDescending(m => m.TotalPrices);
                return View(v);
            }
            return View();
        }
    }
}