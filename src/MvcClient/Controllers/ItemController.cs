using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Authorization;
using MvcClient.Models;
using MvcClient.Services;
using MvcClient.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MvcClient.Controllers
{
    [Authorize(Roles = "Sales, Managers, Administrators")]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ItemController(IItemService itemService, IAuthorizationService authorizationService,
            IIdentityService<Buyer> identityService, IWebHostEnvironment hostEnvironment)
        {
            _identityService = identityService;
            _authorizationService = authorizationService;
            _itemService = itemService;
            webHostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(double minPrice, double maxPrice, string sortOrder, int pageNumber = 1, string ItemCategory = null, string SearchString = null)
        {
            var catalog = await GetViewModel(minPrice, maxPrice, sortOrder, pageNumber, ItemCategory, SearchString);
            return View(catalog);
        }
        public async Task<IActionResult> ItemPaging(double minPrice, double maxPrice, string sortOrder, int pageNumber = 1, string ItemCategory = null, string SearchString = null)
        {
            var catalog = await GetViewModel(minPrice, maxPrice, sortOrder, pageNumber, ItemCategory, SearchString);
            return new JsonResult(catalog);
        }
        private async Task<IndexViewModel> GetViewModel(double minPrice, double maxPrice, string sortOrder, int pageNumber = 1, string ItemCategory = null, string SearchString = null)
        {
            var pageSize = 6;

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole) ||
                                User.IsInRole(Constants.ManagersRole) ||
                                User.IsInRole(Constants.SalesRole);
            var ownerId = User.IsInRole(Constants.SalesRole) ? _identityService.Get(User).Id : null;
            var catalog = await _itemService.GetCatalog(ItemCategory, SearchString, minPrice, maxPrice, sortOrder, isAuthorized, ownerId);

            if (ownerId != null)
                catalog.IsSale = true;
            else catalog.IsSale = false;

            catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);
            catalog.PageIndex = pageNumber;
            catalog.PageTotal = catalog.ItemsPaging.TotalPages;
            return catalog;
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetItem(id);

            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            ItemCategoryViewModel viewModel = new ItemCategoryViewModel();
            viewModel.Item = null;
            viewModel.Categories = await _itemService.GetCategories();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCategoryViewModel viewModel)
        {
            string uniqueFileName = UploadedFile(viewModel);
            Item item = viewModel.Item;
            item.PublishDate = DateTime.Today;
            item.PictureUrl = uniqueFileName;
            if (ModelState.IsValid)
            {
                item.OwnerId = _identityService.Get(User).Id;

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, Operations.Create);
                if (!isAuthorize.Succeeded)
                {
                    return Forbid();
                }

                await _itemService.CreateItem(item);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private string UploadedFile(ItemCategoryViewModel model)
        {
            string uniqueFileName = null;

            if (model.ImageURL != null)
            {
                Console.WriteLine("anime " + model.ImageURL.FileName);
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product/");
                uniqueFileName = model.Item.OwnerId + "_" + model.Item.Name + Path.GetExtension(model.ImageURL.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageURL.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _itemService.GetItem(id);
            ItemCategoryViewModel viewModel = new ItemCategoryViewModel();
            viewModel.Item = item;
            viewModel.Categories = await _itemService.GetCategories();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemCategoryViewModel viewModel)
        {
            Item item = viewModel.Item;
            if (viewModel.ImageURL != null)
            {
                string uniqueFileName = UploadedFile(viewModel);
                item.PictureUrl = uniqueFileName;
            }

            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var itemToUpdate = await _itemService.GetItem(id);

                if (itemToUpdate == null)
                {
                    return NotFound();
                }

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, itemToUpdate, Operations.Update);
                if (!isAuthorize.Succeeded)
                {
                    return Forbid();
                }

                await _itemService.UpdateItem(id, item);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemService.GetItem(id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _itemService.GetItem(id);

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, Operations.Delete);
            if (!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            await _itemService.DeleteItem(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _itemService.GetItem(id);

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, Operations.Approve);
            if (!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            item.ItemStatus = ItemStatus.Approved;
            await _itemService.UpdateItem(id, item);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var item = await _itemService.GetItem(id);

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, Operations.Reject);
            if (!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            item.ItemStatus = ItemStatus.Rejected;
            await _itemService.UpdateItem(id, item);

            return RedirectToAction(nameof(Index));
        }
    }
}