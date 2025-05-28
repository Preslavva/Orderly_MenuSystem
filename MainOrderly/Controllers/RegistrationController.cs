using Microsoft.AspNetCore.Mvc;
using Services;
using MainOrderly.WebApp.ViewModels;
using MSSQL;

namespace MainOrderly.WebApp.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AuthenticationService _authService;
        private readonly OwnerRepository _ownerRepository;

        public RegistrationController(AuthenticationService authService, OwnerRepository ownerRepository)
        {
            _authService = authService;
            _ownerRepository = ownerRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Business/Register.cshtml", new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Business/Register.cshtml", model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View("~/Views/Business/Register.cshtml", model);
            }

            var success = _authService.RegisterOwner(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Phone,
                model.Password,
                model.RestaurantName,
                model.RestaurantEmail,
                model.RestaurantPhone,
                model.RestaurantAddress,
                model.KVK,
                model.Description
            );

            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! You can now log in to manage your restaurant.";
                return RedirectToAction("Login", "BusinessAccount");
            }
            else
            {
                ModelState.AddModelError("", "Registration failed. Email or KVK number may already exist.");
                return View("~/Views/Business/Register.cshtml", model);
            }
        }

        [HttpPost]
        public JsonResult CheckEmailAvailability(string email)
        {
            var ownerExists = _ownerRepository.GetByEmail(email) != null;
            var isAvailable = !ownerExists;
            return Json(new { available = isAvailable });
        }

        [HttpPost]
        public JsonResult CheckKVKAvailability(string kvk)
        {
            var isAvailable = !_ownerRepository.IsKvkExists(kvk);
            return Json(new { available = isAvailable });
        }
    }
}