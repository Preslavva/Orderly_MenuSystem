using MainOrderly.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

namespace MainOrderly.WebApp.Controllers
{
    public class BusinessAccountController : Controller
    {
        private readonly AuthenticationService _authService;

        public BusinessAccountController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("MenuItems", "Manager");
            }
            return View("~/Views/Business/LoginPage.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required");
                return View("~/Views/Business/LoginPage.cshtml");
            }

            var authenticatedUser = _authService.AuthenticateUser(email, password);
            
            if (authenticatedUser == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View("~/Views/Business/LoginPage.cshtml");
            }
            HttpContext.Session.SetAuthenticatedUser(authenticatedUser);
            return RedirectToAction("MenuItems", "Manager");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.ClearAuthenticatedUser();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "BusinessAccount");
        }


    }
}