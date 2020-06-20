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
    public class ShopController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _settings;
        private readonly IItemService _service;
        private readonly IIdentityService<Buyer> _identityService;
        private readonly IUserService _userService;

        public ShopController(IItemService service, IOptions<AppSettings> settings, ILogger<HomeController> logger,
            IIdentityService<Buyer> identityService, IUserService userService)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _userService = userService;
            _identityService = identityService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string itemCategory, string currentFilter,
        string searchString, double minPrice, double maxPrice, string saleId, int pageNumber = 1)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var isAdminOrManager = User.IsInRole(Constants.AdministratorsRole) ||
                User.IsInRole(Constants.ManagersRole);
            var catalog = await _service.GetCatalog(itemCategory, searchString, minPrice, maxPrice, sortOrder, isAdminOrManager, saleId);
            catalog.Sales = await _userService.GetSales();

            int pageSize = 6;
            if (!isAdminOrManager)
            {
                //var userId = _identityService.Get (User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved)
                    .ToList();


            }
            catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);

            DateTime oldDate = DateTime.Today.AddMonths(-3);

            catalog.LatestItems = catalog.Items.Where(m => DateTime.Compare(m.PublishDate, oldDate) > 0).ToList();

            ChangeUriPlaceholder(catalog.Items);

            return View(catalog);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ItemPaging(string itemCategory, string searchString, string sortOrder, double minPrice, double maxPrice,
         string currentFilter, int pageNumber, string saleId)
        {
            int pageSize = 6;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var isAdminOrManager = User.IsInRole(Constants.AdministratorsRole) ||
                User.IsInRole(Constants.ManagersRole);
            var catalog = await _service.GetCatalog(itemCategory, searchString, minPrice, maxPrice, sortOrder, isAdminOrManager, saleId);
            if (!isAdminOrManager)
            {
                // var userId = _identityService.Get (User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved)
                    .ToList();
            }
            catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);

            ChangeUriPlaceholder(catalog.Items);
            catalog.PageTotal = catalog.ItemsPaging.TotalPages;
            catalog.PageIndex = pageNumber;
            return new JsonResult(catalog);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetItem(id);
            var catalog = await _service.GetCatalog(item.Category);
            catalog.item = item;
            catalog.Items = catalog.Items.Where(m => m.Id != item.Id).ToList();
            catalog.Sales = await _userService.GetSales();
            catalog.SellerItems = catalog.Items.Where(m => m.OwnerId == item.OwnerId).ToList();
            ViewData["SaleName"] = catalog.Sales.Where(m => m.UserId.Equals(catalog.item.OwnerId)).Select(m => m.Name).FirstOrDefault();

            return View(catalog);
        }
        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            var item = await _service.GetItem(id);

            return View(item);
        }

        private void ChangeUriPlaceholder(IList<Item> items)
        {
            var baseUri = _settings.ExternalCatalogBaseUrl;

            foreach (var item in items)
            {
                item.PictureUrl = string.IsNullOrEmpty(item.PictureUrl) ? "/images/products/0.png" :
                    item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", baseUri);

            }
        }
    }
}