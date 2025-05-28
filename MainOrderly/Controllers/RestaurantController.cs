using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;
using MainOrderly.WebApp.Extensions;
using MSSQL;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner")]
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly OwnerRepository _ownerRepository;

        public RestaurantController(RestaurantService restaurantService, OwnerRepository ownerRepository)
        {
            _restaurantService = restaurantService;
            _ownerRepository = ownerRepository;
        }

        [HttpGet]
        public IActionResult Index(int id = 0)
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            Restaurant restaurant = null;
            
            if (id != 0)
            {
                restaurant = _restaurantService.GetRestaurantById(id);
            }
            else
            {
                restaurant = _ownerRepository.GetRestaurantByOwnerId(user.UserId);
            }

            if (restaurant != null)
            {
                var restaurantModel = new RestaurantViewModel
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Email = restaurant.Email,
                    PhoneNumber = restaurant.PhoneNumber,
                    Address = restaurant.Address,
                    Description = restaurant.Description,
                    KVK = restaurant.KVK,
                    IsActive=restaurant.IsActive
                };
                return View(restaurantModel);
            }

            return View(new RestaurantViewModel());
        }

        [HttpPost]
        public IActionResult UpdateRestaurant(RestaurantViewModel restaurantViewModel)
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            if (!ModelState.IsValid)
            {
                return View("Index", restaurantViewModel);
            }

            try
            {
                var success = _ownerRepository.UpdateRestaurant(
                    restaurantViewModel.Id,
                    restaurantViewModel.Name,
                    restaurantViewModel.Email,
                    restaurantViewModel.PhoneNumber,
                    restaurantViewModel.Address,
                    restaurantViewModel.Description
                );

                if (success)
                {
                    TempData["SuccessMessage"] = "Restaurant updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update restaurant.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating restaurant: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ArchiveRestaurant()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            try
            {
                var success = _ownerRepository.DeactivateRestaurant(user.RestaurantId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Restaurant has been deactivated.";
                    HttpContext.Session.ClearAuthenticatedUser();
                    return RedirectToAction("Login", "BusinessAccount");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to deactivate restaurant.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deactivating restaurant: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}