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

        private readonly IUserService _userService;

        public AdminController(ILogger<AdminController> logger, IItemService itemService, IOrderService orderService, IUserService userService)
        {
            _logger = logger;
            _itemService = itemService;
            _orderService = orderService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            //get Data
            List<OrderItem> list = new List<OrderItem>();
            var items = await _itemService.GetAll();
            var sales = await _userService.GetSales();

            var orders = await _orderService.GetOrders();
            orders = orders.Where(m => m.Status != OrderStatus.Rejected && m.Status != OrderStatus.Preparing);
            foreach (var order in orders)
                foreach (var item in order.OrderItems)
                    list.Add(item);

            List<LineItem> commonItems = list
                                .GroupBy(cl => cl.ItemId)
                                .Select(cl => new LineItem
                                {
                                    ItemName = cl.First().ItemName,
                                    Total = cl.Sum(c => c.Units),
                                    PictureURL = cl.First().PictureUrl,
                                    OwnerName = (sales.Where(s => s.UserId.Equals(cl.First().OwnerId))) == null ||
                                        (sales.Where(s => s.UserId.Equals(cl.First().OwnerId))).Count() == 0 ? "null" : (sales.Where(s => s.UserId.Equals(cl.First().OwnerId))).FirstOrDefault().Name,
                                    UnitPrice = cl.First().UnitPrice
                                }).ToList();
            commonItems = commonItems.OrderByDescending(c => c.Total).Take(5).ToList();

            DateTime nowDate = DateTime.Now;
            //ViewModel
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
            viewModel.Data = prepareDataChart(orders, nowDate);
            var catalog = await _itemService.GetCatalog();
            viewModel.CommonItems = commonItems;
            return View(viewModel);
        }
        private IList<DataChart> prepareDataChart(IEnumerable<Order> orders, DateTime nowDate)
        {
            var list = new List<DataChart>();
            for (int i = 0; i < 12; ++i)
            {
                DateTime indexDate = nowDate.AddMonths(-11 + i);
                double a = 0;
                a = (from m in orders
                    where m.OrderDate.Year == indexDate.Year && m.OrderDate.Month == indexDate.Month
                    select m.Total).Sum();
                list.Add(new DataChart(indexDate.ToString("MMM") + "-" + indexDate.Year, a));
            }
            return list;
        }
        public IActionResult Seo()
        {
            return View();
        }
        public IActionResult Ecommerce()
        {
            return View();
        }

    }

}