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
		public async Task<IActionResult> Index(string? kvk)
		{

            if (kvk != null)
            {
                var restaurant = _restaurantService.GetRestaurantByKVK(kvk);
                var fonts = await LoadGoogleFontsAsync();
                var restaurantModel = RestaurantViewModel.ConvertToViewModel(restaurant, fonts);
                return View(restaurantModel);
            }
			return View(new RestaurantViewModel()); 
		}

        [HttpGet]
        public async Task<IActionResult> CreateRestaurant()
        {
            var fonts = await LoadGoogleFontsAsync();
            return View(new RestaurantViewModel() { Fonts = fonts });
        }

		[HttpPost]
        public IActionResult RegisterRestaurant(RestaurantViewModel restaurantModel, int ownerId)
        {
            ownerId = 1;
            if(restaurantModel.LogoImage != null)
            {
                restaurantModel.Logo = _restaurantService.ConvertToString(restaurantModel.LogoImage);
            }
            Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantModel);
            _restaurantService.CreateRestaurant(restaurant, ownerId);
			return RedirectToAction("Index", "Restaurant"); // ?
		}

        [HttpPost]
        public IActionResult UpdateRestaurant(RestaurantViewModel restaurantViewModel)
        {
            Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantViewModel);
            _restaurantService.UpdateRepository(restaurant);
            return RedirectToAction();
        }

        [HttpPost]
        public IActionResult ArchiveRestaurant(int id)
        {
            _restaurantService.RemoveRestaurant(id);
            return RedirectToAction();
        }

        [HttpPost]
        public IActionResult LoadOwnerRestaurant(int id)
        {
            _restaurantService.GetRestaurantById(id);
            return RedirectToAction();
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
