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
        public IActionResult Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            viewModel.TotalRevenue = 9999;
            viewModel.CountApproved = 20;
            viewModel.CountRejected = 2;
            viewModel.CountSubmitted = 5;
            return View(viewModel);
        }
        public IActionResult Analysis()
        {
            return View();
        }

    }
}