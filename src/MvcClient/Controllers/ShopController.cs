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

        public ShopController(IItemService service, IOptions<AppSettings> settings, ILogger<HomeController> logger,
            IIdentityService<Buyer> identityService)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _identityService = identityService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string itemCategory, string searchString, string currentFilter, int pageNumber = 1)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var catalog = await _service.GetCatalog(itemCategory, searchString, sortOrder);
            var isAdminOrManager = User.IsInRole(Constants.AdministratorsRole) ||
                User.IsInRole(Constants.ManagersRole);
            int pageSize = 6;
            if (!isAdminOrManager)
            {
                //var userId = _identityService.Get (User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved)
                    .ToList();

                catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);
                Console.WriteLine(catalog.ItemsPaging);
            }

            ChangeUriPlaceholder(catalog.Items);

            return View(catalog);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ItemPaging(string itemCategory, string searchString, string sortOrder, string currentFilter, int pageNumber)
        {
            int pageSize = 6;
            var catalog = await _service.GetCatalog(itemCategory, searchString, sortOrder);
            var isAdminOrManager = User.IsInRole(Constants.AdministratorsRole) ||
                User.IsInRole(Constants.ManagersRole);
            if (!isAdminOrManager)
            {
                //var userId = _identityService.Get (User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved)
                    .ToList();
                catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);
            }

            ChangeUriPlaceholder(catalog.Items);
            catalog.PageTotal = catalog.ItemsPaging.TotalPages;

            return new JsonResult(catalog);
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