using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly string _googleFontsAPIkey;
        public RestaurantController(RestaurantService restaurantService, IConfiguration configuration)
        {
            _restaurantService = restaurantService;
            _googleFontsAPIkey = configuration["GoogleFonts:ApiKey"];
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

		[HttpPost]
        public IActionResult RegisterRestaurant(RestaurantViewModel restaurantModel)
        {
            try
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
        }
    }
}
