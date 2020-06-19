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
    [Authorize(Roles = "Sales, Managers, Administrators")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;

        public AdminController(ILogger<AdminController> logger, IItemService itemService, IOrderService orderService)
        {
            _logger = logger;
            _itemService = itemService;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            List<OrderItem> list = new List<OrderItem>();
            var orders = await _orderService.GetOrders();
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    list.Add(item);

                }
            }
            List<LineItem> commonItems = list
                                .GroupBy(l => l.ItemName)
                                .Select(cl => new LineItem
                                {
                                    ItemName = cl.First().ItemName,
                                    Total = cl.Sum(c => c.Units),
                                    PictureURL = cl.First().PictureUrl,
                                    UnitPrice = cl.First().UnitPrice
                                }).ToList();
            commonItems = commonItems.OrderByDescending(c => c.Total).ToList();
            var top = from m in list
                      group m by m.ItemName into g
                      select new { id = g.Key, cnt = g.Count() };
            Console.WriteLine("aaa " + top.First().id + " " + top.First().cnt);
            var items = await _itemService.GetAll();
            DashboardViewModel viewModel = new DashboardViewModel();
            viewModel.TotalRevenue = (from m in orders
                                      select m.Total).Sum();
            viewModel.CountApproved = (from m in items
                                       where m.ItemStatus == ItemStatus.Approved
                                       select m).Count();
            viewModel.CountRejected = (from m in items
                                       where m.ItemStatus == ItemStatus.Rejected
                                       select m).Count();
            viewModel.CountSubmitted = (from m in items
                                        where m.ItemStatus == ItemStatus.Submitted
                                        select m).Count();
            viewModel.Data = prepareDataChart();
            var catalog = await _itemService.GetCatalog(null, null, 0, 0, null);
            viewModel.CommonItems = commonItems;
            return View(viewModel);
        }
        public IActionResult Seo()
        {
            return View();
        }
        public IActionResult Ecommerce()
        {
            return View();
        }
        private IList<DataChart> prepareDataChart()
        {
            var list = new List<DataChart>();
            list.Add(new DataChart("January", 10));
            list.Add(new DataChart("February", 15));
            list.Add(new DataChart("March", 18));
            list.Add(new DataChart("April", 12));
            list.Add(new DataChart("May", 27));
            list.Add(new DataChart("June", 15));
            list.Add(new DataChart("July", 30));
            list.Add(new DataChart("August", 25));
            list.Add(new DataChart("September", 25));
            list.Add(new DataChart("October", 8));
            list.Add(new DataChart("November", 55));
            list.Add(new DataChart("December", 35));
            return list;
        }

    }

}