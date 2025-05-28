using Microsoft.AspNetCore.Mvc;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Services;

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
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user != null)
            {
                return RedirectBasedOnRole(user);
            }
            return View("~/Views/Business/LoginPage.cshtml", new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Business/LoginPage.cshtml", model);
            }

            var user = _authService.Login(model.Email, model.Password);
            if (user != null)
            {
                HttpContext.Session.SetAuthenticatedUser(user);
                return RedirectBasedOnRole(user);
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View("~/Views/Business/LoginPage.cshtml", model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.ClearAuthenticatedUser();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectBasedOnRole(AuthenticatedUser user)
        {
            if (user.UserType == "Owner")
            {
                return RedirectToAction("Dashboard", "Owner");
            }
            else if (user.Roles.Contains("Manager"))
            {
                return RedirectToAction("MenuItems", "Manager");
            }
            else if (user.Roles.Contains("Chef"))
            {
                return RedirectToAction("Dashboard", "Kitchen");
            }
            else if (user.Roles.Contains("Waiter"))
            {
                return RedirectToAction("Orders", "Waiter");
            }
            else
            {
                return RedirectToAction("MenuItems", "Manager");
            }
        }
    }
}