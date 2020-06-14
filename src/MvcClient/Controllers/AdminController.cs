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
    [Authorize(Roles = "Managers, Administrators")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IItemService _itemService;

        public AdminController(ILogger<AdminController> logger, IItemService itemService)
        {
            _logger = logger;
            _itemService = itemService;
        }
        public async Task<IActionResult> Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            viewModel.TotalRevenue = 9999;
            viewModel.CountApproved = 20;
            viewModel.CountRejected = 2;
            viewModel.CountSubmitted = 5;
            viewModel.Data = prepareDataChart();
            var catalog = await _itemService.GetCatalog(null, null, 0, 0, null);
            viewModel.CommonItems = catalog.Items;
            return View(viewModel);
        }
        public IActionResult Analysis()
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