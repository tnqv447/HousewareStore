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
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var pageSize = 6;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers();
            viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
            return View(viewModel);
        }
        public async Task<IActionResult> Create()
        {
            UserChildViewModel viewModel = new UserChildViewModel();
            viewModel.User = null;
            // viewModel.RoleList = _service.  
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserChildViewModel viewModel)
        {
            User user = viewModel.User;

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
            UserChildViewModel viewModel = new UserChildViewModel();
            viewModel.User = user;
            // viewModel.RoleList = _service. 
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserChildViewModel viewModel)
        {
            var user = viewModel.User;


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

                var isAuthorize = await _authorizationService.AuthorizeAsync(User, userToUpdate, ItemOperations.Update);
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

            var isAuthorize = await _authorizationService.AuthorizeAsync(User, user, ItemOperations.Delete);
            if (!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            await _service.DeleteUser(id);

            return RedirectToAction(nameof(Index));
        }
    }

}