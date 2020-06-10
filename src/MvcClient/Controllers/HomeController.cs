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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _settings;
        private readonly IItemService _service;
        private readonly IIdentityService<Buyer> _identityService;

        public HomeController(IItemService service, IOptions<AppSettings> settings, ILogger<HomeController> logger,
            IIdentityService<Buyer> identityService)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _identityService = identityService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var catalog = await _service.GetCatalog("", "",10,100000000,"");
            var isAdminOrManager = User.IsInRole(Constants.AdministratorsRole) ||
                User.IsInRole(Constants.ManagersRole);

            if (!isAdminOrManager)
            {
                //var userId = _identityService.Get (User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved)
                    .ToList();
            }

            ChangeUriPlaceholder(catalog.Items);

            return View(catalog);
        }
        [AllowAnonymous]
        public IActionResult SearchCategory(string itemCategory, string searchString){    
            // var uri = new Uri.Action("Index","Shop");
            return RedirectToAction("Index","Shop", new{itemCategory = itemCategory, searchString= searchString});
        }
        public IActionResult Privacy()
        {
            return RedirectToAction("Index");
            //Response.Redirect("Index");
            //return View();
        }
        [AllowAnonymous]
        
        
        public IActionResult Contact()
        {
            return View();
        }
        // public IActionResult Logout()
        // {
        //     return SignOut("Cookies", "oidc");
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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