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
    [Authorize(Roles = "Administrators")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppSettings _settings;
        private readonly IUserService _service;
        private readonly IAuthorizationService _authorizationService;

        public UserController(ILogger<UserController> logger, IOptions<AppSettings> settings, IUserService service, IAuthorizationService authorizationService)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
            _authorizationService = authorizationService;
        }
        public async Task<IActionResult> Index(string searchName = null, string itemRole = null, int pageNumber = 1, string sortOrder = null)
        {
            var pageSize = 3;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers();
            viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
            viewModel.PageIndex = pageNumber;
            viewModel.PageTotal = viewModel.UsersPaging.TotalPages;
            return View(viewModel);
        }
        public async Task<IActionResult> UserPaging(string searchName, string itemRole, int pageNumber = 1, string sortOrder = null)
        {
            var pageSize = 3;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers();
            viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
            viewModel.PageIndex = pageNumber;
            viewModel.PageTotal = viewModel.UsersPaging.TotalPages;
            return new JsonResult(viewModel);
        }
        public IActionResult Create()
        {
            return View();
        }
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
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _service.GetUser(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User user)
        {
            user.Name = user.GivenName + " " + user.FamilyName;
            if (id != user.UserId)
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