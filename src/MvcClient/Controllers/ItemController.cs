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

        public async Task<IActionResult> Index(double minPrice, double maxPrice, int pageNumber = 1, string ItemCategory = null, string SearchString = null)
        {
            // double minPrice = 0, double maxPrice = 999999, string sortOrder = "Name"
            var pageSize = 6;

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole) ||
                                User.IsInRole(Constants.ManagersRole) ||
                                User.IsInRole(Constants.SalesRole);
            var ownerId = User.IsInRole(Constants.SalesRole) ? _identityService.Get(User).Id : null;
            var catalog = await _itemService.GetCatalog(ItemCategory, SearchString, minPrice, maxPrice, null, isAuthorized, ownerId);

            if (User.IsInRole(Constants.AdministratorsRole))
            {
                catalog.UserRole = Constants.AdministratorsRole;
            }
            else if (User.IsInRole(Constants.ManagersRole))
            {
                catalog.UserRole = Constants.ManagersRole;
            }
            else if (User.IsInRole(Constants.SalesRole))
            {
                catalog.UserRole = Constants.SalesRole;
            }
            else
            {
                catalog.UserRole = Constants.UsersRole;
            }


            catalog.OwnerId = _identityService.Get(User).Id;

            if (!isAuthorized)
            {
                var userId = _identityService.Get(User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved || m.OwnerId == userId)

                    .ToList();
            }
            // Console.WriteLine("owner " + isAuthorized + " " + catalog.OwnerId);
            catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);
            catalog.PageIndex = pageNumber;
            catalog.PageTotal = catalog.ItemsPaging.TotalPages;
            return View(catalog);
        }
        public async Task<IActionResult> ItemPaging(double minPrice, double maxPrice, string sortOrder, int pageNumber = 1, string ItemCategory = null, string SearchString = null)
        {
            // string sortOrder = "Name"
            var pageSize = 6;

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole) ||
                                User.IsInRole(Constants.ManagersRole);
            var catalog = await _itemService.GetCatalog(ItemCategory, SearchString, minPrice, maxPrice, sortOrder, isAuthorized);

            if (User.IsInRole(Constants.AdministratorsRole))
            {
                catalog.UserRole = Constants.AdministratorsRole;
            }
            else if (User.IsInRole(Constants.ManagersRole))
            {
                catalog.UserRole = Constants.ManagersRole;
            }
            else if (User.IsInRole(Constants.SalesRole))
            {
                catalog.UserRole = Constants.SalesRole;
            }
            else
            {
                catalog.UserRole = Constants.UsersRole;
            }


            if (!isAuthorized)
            {
                var userId = _identityService.Get(User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved || m.OwnerId == userId)
                    .ToList();
                catalog.OwnerId = userId;
            }
            catalog.ItemsPaging = PaginatedList<Item>.Create(catalog.Items, pageNumber, pageSize);
            catalog.PageIndex = pageNumber;
            catalog.PageTotal = catalog.ItemsPaging.TotalPages;
            return new JsonResult(catalog);
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
        public string UploadFileName(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            fileName = name + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-tt") + extension;
            return fileName;
        }
        private string UploadedFile(ItemCategoryViewModel model)
        {
            string uniqueFileName = null;

            if (model.ImageURL != null)
            {
                Console.WriteLine("anime " + model.ImageURL.FileName);
                string uploadFileName = UploadFileName(model.ImageURL.FileName);
                Console.WriteLine("anime " + uploadFileName);
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadFileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Console.WriteLine("anime " + uploadsFolder);
                // Console.WriteLine("anime " + filePath);
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