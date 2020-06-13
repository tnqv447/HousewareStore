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
    public class AdminController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppSettings _settings;

        public AdminController(ILogger<UserController> logger, AppSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }
    }
}