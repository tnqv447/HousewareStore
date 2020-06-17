using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;

namespace MvcClient.Controllers
{

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppSettings _settings;
        private readonly IUserService _service;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserController(ILogger<UserController> logger, IOptions<AppSettings> settings, IUserService service,
                            IAuthorizationService authorizationService, IIdentityService<Buyer> identityService,
                             IWebHostEnvironment hostEnvironment)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _authorizationService = authorizationService;
            _identityService = identityService;
            webHostEnvironment = hostEnvironment;
        }
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Index(string searchName = null, string itemRole = null, int pageNumber = 1, string sortOrder = null, string sortBy = null)
        {
            var viewModel = await GetViewModel(searchName, itemRole, pageNumber, sortOrder, sortBy);
            return View(viewModel);
        }
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> UserPaging(string searchName = null, string itemRole = null, int pageNumber = 1, string sortOrder = null, string sortBy = null)
        {
            var viewModel = await GetViewModel(searchName, itemRole, pageNumber, sortOrder, sortBy);
            return new JsonResult(viewModel);
        }
        [Authorize(Roles = "Administrators")]
        private async Task<UserViewModel> GetViewModel(string searchName = null, string itemRole = null, int pageNumber = 1, string sortOrder = null, string sortBy = null)
        {
            SortOrder SortOrder1 = SortOrder.Ascending;
            switch (sortOrder)
            {
                case "Ascending": SortOrder1 = SortOrder.Ascending; break;
                case "Descending": SortOrder1 = SortOrder.Descending; break;
                default: SortOrder1 = SortOrder.Ascending; break;
            }
            SortType SortType1 = SortType.FullName;
            switch (sortBy)
            {
                case "FullName": SortType1 = SortType.FullName; break;
                case "Role": SortType1 = SortType.Role; break;
                default: SortType1 = SortType.FullName; break;
            }
            var pageSize = 6;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers(itemRole, searchName, null, SortType1, SortOrder1);
            if (viewModel.Users == null)
            {
                viewModel.UsersPaging = null;
            }
            else
            {
                viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
                viewModel.PageIndex = pageNumber;
                viewModel.PageTotal = viewModel.UsersPaging.TotalPages;
            }
            return viewModel;
        }
        [Authorize(Roles = "Sales, Managers, Administrators")]
        public IActionResult ProfileAdmin()
        {
            var user = _identityService.Get(User);//
            return View(user);
        }


        [Authorize(Roles = "Users")]
        public IActionResult Account()
        {
            BuyerViewModel bvm = new BuyerViewModel();

            var buyer = _identityService.Get(User);
            bvm.buyer = buyer;
            return View(bvm);
        }
        [Authorize(Roles = "Users")]
        public IActionResult Profile()
        {
            BuyerViewModel bvm = new BuyerViewModel();

            var buyer = _identityService.Get(User);
            bvm.buyer = buyer;
            return View(bvm);
        }
        [Authorize(Roles="Users")]
        [HttpPost]
        public async Task<IActionResult> Profile(BuyerViewModel bvm){
            string id = _identityService.Get(User).Id;
            var user = await _service.GetUser(id);
            Console.WriteLine(bvm.buyer.Address.Country);
            if(user == null){
                return NotFound();
            }
            if(bvm.buyer == null){
                return NotFound();
            }
            else{
                user.Name = bvm.buyer.FirstName + " " + bvm.buyer.LastName;
                user.PhoneNumber = bvm.buyer.PhoneNumber;
                user.Email = bvm.buyer.Email;
                user.Address = bvm.buyer.Address;
            }
            await _service.UpdateUser(id,user);
            return View(bvm);
        }
        [Authorize(Roles = "Administrators")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Administrators")]
        [HttpPost]

        public async Task<IActionResult> Create(User user)
        {
            user.Name = user.GivenName + " " + user.FamilyName;
            user.Role = "Managers";
            if (user.Password == null || user.Password.Trim().Equals(""))
                user.Password = "Pass123$";
            if (String.IsNullOrEmpty(user.ImageURL.FileName))
                user.PictureUrl = "default_avatar.png";
            else user.PictureUrl = UploadedFile(user);

            if (ModelState.IsValid)
            {
                await _service.CreateUser(user);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private string UploadedFile(User user)
        {
            string uniqueFileName = null;

            if (user.ImageURL != null)
            {
                Console.WriteLine("anime " + user.ImageURL.FileName);
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/user/");
                uniqueFileName = user.UserName + Path.GetExtension(user.ImageURL.FileName);
                Console.WriteLine("anime " + uniqueFileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    user.ImageURL.CopyTo(fileStream);
                    Console.WriteLine("anime Done ");
                }
            }

            return uniqueFileName;
        }
        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _service.GetUser(id);
            return View(user);
        }
        [Authorize(Roles = "Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User user)
        {
            if (String.IsNullOrEmpty(user.PictureUrl))
                user.PictureUrl = "default_avatar.png";
            user.Role = "Managers";
            user.Name = user.GivenName + " " + user.FamilyName;
            if (!id.Equals(user.UserId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userToUpdate = await _service.GetUser(id);

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, userToUpdate, Operations.Update);
                if (!isAuthorize.Succeeded)
                {
                    return Forbid();
                }

                await _service.UpdateUser(id, user);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _service.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [Authorize(Roles = "Administrators")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _service.GetUser(id);

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, user, Operations.Delete);
            if (!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            await _service.DeleteUser(id);

            return RedirectToAction(nameof(Index));
        }
    }

}