using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Authorization;
using MvcClient.Models;
using MvcClient.Services;
using MvcClient.ViewModels;

namespace MvcClient.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;

        public ItemController(IItemService itemService, IAuthorizationService authorizationService,
            IIdentityService<Buyer> identityService)
        {
            _identityService = identityService;
            _authorizationService = authorizationService;
            _itemService = itemService;
        }

        public async Task<IActionResult> Index(string itemGenre, string searchString,string sortOrder)
        {
            var catalog = await _itemService.GetCatalog(itemGenre, searchString,sortOrder);

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole) ||
                                User.IsInRole(Constants.ManagersRole);

            if (!isAuthorized)
            {
                var userId = _identityService.Get(User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved || m.OwnerId == userId)
                    .ToList();
            }

            return View(catalog);
        }


        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetItem(id);

            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = await _itemService.GetCreateItem();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                item.OwnerId = _identityService.Get(User).Id;

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, ItemOperations.Create);
                if (!isAuthorize.Succeeded)
                {
                    return Forbid();
                }

                await _itemService.CreateItem(item);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _itemService.GetItem(id);

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
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

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, itemToUpdate, ItemOperations.Update);
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

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, ItemOperations.Delete);
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

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, ItemOperations.Approve);
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

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, ItemOperations.Reject);
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