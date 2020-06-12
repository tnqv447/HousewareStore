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

        public UserController(ILogger<UserController> logger, IOptions<AppSettings> settings, IUserService service)
        {
            _settings = settings.Value;
            _service = service;
            _logger = logger;
        }
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var pageSize = 6;
            UserViewModel viewModel = new UserViewModel();
            viewModel.Users = await _service.ManageUsers();
            viewModel.UsersPaging = PaginatedList<User>.Create(viewModel.Users, pageNumber, pageSize);
            return View(viewModel);
        }
    }

}