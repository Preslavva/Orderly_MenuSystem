using Microsoft.AspNetCore.Mvc;

namespace MainOrderly.WebApp.Controllers
{
    public class BusinessAccountController : Controller
    {
        public IActionResult Login()
        {
            return View("~/Views/Business/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required");
                return View("~/Views/Business/Login.cshtml");
            }

            return RedirectToAction("Index", "Business");
        }

        public IActionResult Register()
        {
            return View("~/Views/Business/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string phoneNumber)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required");
                return View("~/Views/Business/Register.cshtml");
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Business");
        }
    }
}