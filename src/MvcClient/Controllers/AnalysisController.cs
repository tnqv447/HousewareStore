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
namespace MvcClient.Controllers{
    public class AnalysisController : Controller{
        private readonly ILogger<AdminController> _logger;
        private readonly IAnalysisService _analysisService;
        public AnalysisController(ILogger<AdminController> logger, IAnalysisService analysisService){
            _logger = logger;
            _analysisService = analysisService;
        }
        [Authorize(Roles="Administrators, Sales, Managers")]
        public async Task<IActionResult> IndexAsync()
        {
            var indexView = await _analysisService.CountAllSales();
            return View(indexView);
        }
        [Authorize(Roles="Administrator, Managers")]
        public async Task<IActionResult> Sale(string id, string saleName){
            // Console.WriteLine("\n asdasda: "+sale.UserId);
            var v = await _analysisService.CountItemsBySalesAsync(id);
            return View(v);
        }
    }
}