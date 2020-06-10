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
using System.Drawing;

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

        public async Task<IActionResult> Index(string itemGenre, string searchString,double minPrice,double maxPrice, string sortOrder)
        {
            var catalog = await _itemService.GetCatalog(itemGenre, searchString, minPrice, maxPrice, sortOrder);

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole) ||
                                User.IsInRole(Constants.ManagersRole);

            if (!isAuthorized)
            {
                var userId = _identityService.Get(User).Id;
                catalog.Items = catalog.Items
                    .Where(m => m.ItemStatus == ItemStatus.Approved || m.OwnerId == userId)
                    .ToList();
            }
            //ok
            return View(catalog);
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
        public async Task<IActionResult> Create(Item item)
        {
            Console.WriteLine("hohoho " + item.PictureUrl); // cái này console hiện ở đâu vậy  cái này có mỗi cái tên image, thì đúng r input nó láy tên image thôi, xong nó tải file ảnh về thư mục của mình mà, tải sao??, thì mình set location đẻ save
            string fullPath = Path.GetFullPath(item.PictureUrl); //lấy tuyệt đối, cơ mà ko hiểu sao tuyêt đối ở đây lại là từ folder của mình
            //ok nha tự làm nha, mà search mvc hoặc .net core nha, tùy thô cái warm là do t ẩn mấy cái hàm create để kt nó có save file trước
            Console.WriteLine("hohoho " + fullPath);// C:\Users\Administrator\Desktop\HousewareStore\src\MvcClient\background.jpg
            // full pat này bị sai
            // string folderPath = Server.MapPath("~/img/product/");
            string folderPath = "~/img/product/";
            string imagePath = folderPath + item.PictureUrl;

            Image originalImage = Image.FromFile(item.PictureUrl);// can full path
            // string filePath = AppDomain.CurrentDomain.BaseDirectory + savedName;
            originalImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);// save

            // MemoryStream ms = new MemoryStream(data);
            // System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            // img.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            // if (ModelState.IsValid)
            // {
            //     item.OwnerId = _identityService.Get(User).Id;

            //     var isAuthorize = await _authorizationService.AuthorizeAsync(User, item, ItemOperations.Create);
            //     if (!isAuthorize.Succeeded)
            //     {
            //         return Forbid();
            //     }

            //     await _itemService.CreateItem(item);

            //     return RedirectToAction(nameof(Index)); đang fix ma đang fix cái này mà nên sao t pull đc, chả lẽ muốn gửi bug lên github, cái bug của m chỉ khi nào t xài thôi, chứ có ảnh hưởng gì t đâu thì xong t sẽ pull sau, đéo 
            // }
            //t mới làm ajax + pagination bao ngon, pull cái xài thử m, khi t fix xong cái này đã, chứ đal
            return View();
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