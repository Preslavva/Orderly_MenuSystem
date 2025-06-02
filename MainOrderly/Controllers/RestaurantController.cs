using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly string _googleFontsAPIkey;
        public RestaurantController(RestaurantService restaurantService, IConfiguration configuration)
        private readonly OwnerRepository _ownerRepository;

        public RestaurantController(RestaurantService restaurantService, OwnerRepository ownerRepository)
        {
            _restaurantService = restaurantService;
            _googleFontsAPIkey = configuration["GoogleFonts:ApiKey"];
            _restaurantService = restaurantService;
            _ownerRepository = ownerRepository;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
            var fonts = await LoadGoogleFontsAsync();
            int ownerId = HttpContext.Session.GetAuthenticatedUser().Id;
            Restaurant restaurant = _restaurantService.GetOwnerRestaurant(ownerId);
            if (restaurant != null)
            {
                var restaurantModel = RestaurantViewModel.ConvertToViewModel(restaurant, fonts);
                return View(restaurantModel);
            }
			return View(new RestaurantViewModel() { Fonts = fonts}); 
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

        [HttpGet]
        public async Task<IActionResult> CreateRestaurant()
        {
            var fonts = await LoadGoogleFontsAsync();
            Restaurant restaurant = _restaurantService.GetOwnerRestaurant(1);
            if (restaurant != null)
            {
                RestaurantViewModel restaurantModel = RestaurantViewModel.ConvertToViewModel(restaurant, fonts);
                return View(restaurantModel);
            }
            return View(new RestaurantViewModel() { Fonts = fonts });
        }
            return View(new RestaurantViewModel());
        }

		[HttpPost]
        public IActionResult RegisterRestaurant(RestaurantViewModel restaurantModel)
        [HttpPost]
        public IActionResult UpdateRestaurant(RestaurantViewModel restaurantViewModel)
        {
            try
            var user = HttpContext.Session.GetAuthenticatedUser();
            if (user == null) return RedirectToAction("Login", "BusinessAccount");

            if (!ModelState.IsValid)
            {
                int ownerId = HttpContext.Session.GetAuthenticatedUser().Id;
                if (restaurantModel.LogoImage != null)
                {
                    restaurantModel.Logo = _restaurantService.ConvertToString(restaurantModel.LogoImage);
                }
                Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantModel);
                _restaurantService.CreateRestaurant(restaurant, ownerId);
                HttpContext.Session.SetString("KVK", restaurant.KVK);
                TempData["SuccessMessage"] = "Restaurant was successfully registered!";
            }
            catch(ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("CreateRestaurant", "Restaurant");
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
        public IActionResult UpdateRestaurant(RestaurantViewModel restaurantViewModel)
        {
            try
            {
                if (restaurantViewModel.LogoImage != null)
                {
                    restaurantViewModel.Logo = _restaurantService.ConvertToString(restaurantViewModel.LogoImage);
                }
                Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantViewModel);
                _restaurantService.UpdateRepository(restaurant);
                TempData["SuccessMessage"] = "Restaurant was successfully updated!";
            }
            catch(ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Restaurant");
        }

        [HttpPost]
        public IActionResult ArchiveRestaurant(int id)
        {
            _restaurantService.RemoveRestaurant(id);
            return RedirectToAction("CreateRestaurant", "Restaurant"); 
        }
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

        private async Task<List<SelectListItem>> LoadGoogleFontsAsync()
        {
            var url = $"https://www.googleapis.com/webfonts/v1/webfonts?key={_googleFontsAPIkey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);

            var json = System.Text.Json.JsonDocument.Parse(response);
            var items = json.RootElement.GetProperty("items");

            var fonts = new List<SelectListItem>();

            foreach (var font in items.EnumerateArray())
            {
                var family = font.GetProperty("family").GetString();
                fonts.Add(new SelectListItem { Text = family, Value = family.Replace(" ", "+") });
            }

            return fonts;
            return RedirectToAction("Index");
        }
    }
}