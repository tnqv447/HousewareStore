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

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppSettings _settings;
        private readonly IUserService _service;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;

        public UserController(ILogger<UserController> logger, IOptions<AppSettings> settings, IUserService service,
                            IAuthorizationService authorizationService, IIdentityService<Buyer> identityService)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _authorizationService = authorizationService;
            _identityService = identityService;
        }
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Index(string searchName = null, string itemRole = null, int pageNumber = 1, string sortOrder = null)
        {
            var viewModel = await GetViewModel(searchName, itemRole, pageNumber, sortOrder);
            return View(viewModel);
        }
        [Authorize(Roles = "Users")]
        public async Task<IActionResult> Account()
        {
            var buyer = _identityService.Get(User);
            var user = await _service.GetUser(buyer.Id);
            return View(user);
        }
        [Authorize(Roles = "Users")]
        public async Task<IActionResult> Profile()
        {
            var buyer = _identityService.Get(User);
            var user = await _service.GetUser(buyer.Id);
            return View(user);
        }
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> UserPaging(string searchName, string itemRole, int pageNumber = 1, string sortOrder = null)
        {
            var viewModel = await GetViewModel(searchName, itemRole, pageNumber, sortOrder);
            return new JsonResult(viewModel);
        }
        [Authorize(Roles = "Administrators")]
        private async Task<UserViewModel> GetViewModel(string searchName, string itemRole, int pageNumber = 1, string sortOrder = null)
        {
            var pageSize = 6;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers(itemRole);
            viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
            viewModel.PageIndex = pageNumber;
            viewModel.PageTotal = viewModel.UsersPaging.TotalPages;
            return viewModel;
        }
        [Authorize(Roles = "Administrators")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            user.Name = user.GivenName + " " + user.FamilyName;
            user.Role = "Managers";
            if (String.IsNullOrEmpty(user.PictureUrl))
                user.PictureUrl = "default_avatar.png";
            if (ModelState.IsValid)
            {
                await _service.CreateUser(user);
                return RedirectToAction(nameof(Index));
            }
            return View();
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
            Console.WriteLine("Hemmll" + user.ToString());
            if (!id.Equals(user.UserId))
            {
                return BadRequest("Ids is not match");
                // return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userToUpdate = await _service.GetUser(id);

                if (userToUpdate == null)
                {
                    return BadRequest("ModelState is inValid");
                    // return NotFound();
                }

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, userToUpdate, Operations.Update);
                if (!isAuthorize.Succeeded)
                {
                    return BadRequest("Not isAuthorize");
                    // return Forbid();
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