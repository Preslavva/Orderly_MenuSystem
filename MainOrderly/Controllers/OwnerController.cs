using Microsoft.AspNetCore.Mvc;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.ViewModels;
using MSSQL;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner")]
    public class OwnerController : Controller
    {
        private readonly OwnerRepository _ownerRepository;
        private readonly RestaurantService _restaurantService;

        public OwnerController(OwnerRepository ownerRepository, RestaurantService restaurantService)
        {
            _ownerRepository = ownerRepository;
            _restaurantService = restaurantService;
        }

        public IActionResult Dashboard()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var restaurant = _ownerRepository.GetRestaurantByOwnerId(user.UserId);
            var owner = _ownerRepository.GetById(user.UserId);

            if (restaurant == null)
            {
                TempData["ErrorMessage"] = "Restaurant not found!";
                return RedirectToAction("Login", "BusinessAccount");
            }

            var model = new OwnerDashboardViewModel
            {
                Owner = new OwnerViewModel
                {
                    Id = owner.Id,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName,
                    Email = owner.Email,
                    Phone = owner.Phone
                },
                Restaurant = new RestaurantViewModel
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Email = restaurant.Email,
                    PhoneNumber = restaurant.PhoneNumber,
                    Address = restaurant.Address,
                    KVK = restaurant.KVK,
                    Description = restaurant.Description,
                    IsActive = restaurant.IsActive
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditRestaurant()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var restaurant = _ownerRepository.GetRestaurantByOwnerId(user.UserId);
            if (restaurant == null) return NotFound();

            var model = new RestaurantViewModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Email = restaurant.Email,
                PhoneNumber = restaurant.PhoneNumber,
                Address = restaurant.Address,
                KVK = restaurant.KVK,
                Description = restaurant.Description,
                IsActive = restaurant.IsActive,
                Logo = restaurant.Logo != null ? Convert.ToBase64String(restaurant.Logo) : "",
                Font = restaurant.Font ?? "",
                ColorButtons = restaurant.ColorButtons ?? "",
                ColorDeafult = restaurant.ColorDefault ?? "",
                ColorBackground = restaurant.ColorBackground ?? "",
                Fonts = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>() 
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditRestaurant(RestaurantViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var success = _ownerRepository.UpdateRestaurant(
                model.Id,
                model.Name,
                model.Email,
                model.PhoneNumber,
                model.Address,
                model.Description
            );

            if (success)
            {
                TempData["SuccessMessage"] = "Restaurant information updated successfully!";
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update restaurant information.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var owner = _ownerRepository.GetById(user.UserId);
            if (owner == null) return NotFound();

            var model = new OwnerViewModel
            {
                Id = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Email = owner.Email,
                Phone = owner.Phone
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditProfile(OwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var success = _ownerRepository.UpdateOwner(
                model.Id,
                model.FirstName,
                model.LastName,
                model.Email,
                model.Phone
            );

            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                user.FullName = $"{model.FirstName} {model.LastName}";
                user.Email = model.Email;
                HttpContext.Session.SetAuthenticatedUser(user);
                
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DeactivateRestaurant()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var success = _ownerRepository.DeactivateRestaurant(user.RestaurantId);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Restaurant has been deactivated successfully.";
                HttpContext.Session.ClearAuthenticatedUser();
                return RedirectToAction("Login", "BusinessAccount");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to deactivate restaurant.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public IActionResult ConfirmDeactivate()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var restaurant = _ownerRepository.GetRestaurantByOwnerId(user.UserId);
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult ActivateRestaurant()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            var success = _ownerRepository.ActivateRestaurant(user.RestaurantId);
    
            if (success)
            {
                TempData["SuccessMessage"] = "Restaurant has been activated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to activate restaurant.";
            }
    
            return RedirectToAction("Dashboard");
        }
    }
}