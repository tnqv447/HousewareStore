using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcClient.Controllers
{
    public class AccountController : Controller
    {
        // [Authorize]
        // public IActionResult Login()
        // {
        //     return View("~/Views/Home/Index.cshtml");
        // }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        
    }
}