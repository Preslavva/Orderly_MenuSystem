using System.Diagnostics;
using MainOrderly.WebApp.Helpers;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartService _cartService;
        private readonly MenuService _menuService;
        private readonly NutritionService _nutritionService;
        private readonly AllergenService _allergenService;
        private readonly IngredientService _ingredientService;
        private readonly TableService _tableService;

        private static Dictionary<int, string> _tableGuidMap = new();

        public HomeController(ILogger<HomeController> logger, CartService cartService, 
            MenuService menuService, NutritionService nutritionService, 
            AllergenService allergenService, IngredientService ingredientService, 
            TableService tableService)
        {
            _logger = logger;
            _cartService = cartService;
            _menuService = menuService;
            _tableService = tableService;
            _nutritionService = nutritionService;
            _allergenService = allergenService;
            _ingredientService = ingredientService;
        }

        public IActionResult LoadingPage(string token,int restaurantId)
        {
          
            Table table = _tableService.GetTableByToken(token, restaurantId);

            if (table == null)
            {
                return NotFound("Invalid token.");
            }

            HttpContext.Session.SetInt32("TableId", table.Id);
            HttpContext.Session.SetInt32("RestaurantId", restaurantId);
  
            return View("~/Views/loading.cshtml");
        }

        [HttpGet]
        public IActionResult Index(int restaurantId,Category? category = null, string token = "", string searchTerm = "")
        {
            ViewData["Page"] = "Index";
            
            if (!string.IsNullOrEmpty(token))
            {
                var table = _tableService.GetTableByToken(token, restaurantId);
                if (table != null)
                {
                    HttpContext.Session.SetInt32("TableId", table.Id);
                    HttpContext.Session.SetInt32("RestaurantId", restaurantId);
                }
            }

            int? tableId = HttpContext.Session.GetInt32("TableId");

            if (tableId.HasValue)
            {
                if (!_tableGuidMap.ContainsKey(tableId.Value))
                {
                    _tableGuidMap[tableId.Value] = Guid.NewGuid().ToString();
                }

                string tableGuid = _tableGuidMap[tableId.Value];

                Response.Cookies.Append(
                    "TableGuid",
                    tableGuid,
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true
                    });
            }

            if (!Request.Cookies.ContainsKey("SessiondID"))
            {
                string sessionID = Guid.NewGuid().ToString();
                Response.Cookies.Append("SessiondID", sessionID,
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true
                    });
            }

            List<MenuItem> allMenuItems = _menuService.LoadMenuItems(restaurantId);

            if (!category.HasValue)
            {
                category = Category.Starters;
            }

            ViewBag.AllCategories = allMenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allMenuItems = allMenuItems
                    .Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (category.HasValue)
            {
                allMenuItems = allMenuItems
                    .Where(m => m.Category == category.Value)
                    .ToList();
            }

            var menuItemViewModel = MappingHelper.ConvertToViewModels(allMenuItems);
            TempData["CartCount"] = _cartService.GetCartCount();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_MenuItemList", menuItemViewModel);
            }

            return View(menuItemViewModel);
        }

        [HttpGet]
        public IActionResult GetItemInfo(int id, Category category)
        {
            int restaurantId = HttpContext.Session.GetInt32("RestaurantId") ?? 1;
            
            var menuItem = _menuService.GetMenuItem(id, restaurantId);
            var menuItemViewModel = MenuItemViewModel.ConvertToViewModel(menuItem);

            var nutritions = _nutritionService.GetNutritionForMenuItem(id, restaurantId);
            var nutritionViewModels = nutritions
                .Select(NutritionViewModel.ConvertToViewModel)
                .ToList();

            var allergens = _allergenService.GetAllergenForMenuItem(id, restaurantId);
            var allergenViewModel = allergens
                .Select(AllergenViewModel.ConvertToViewModel)
                .ToList();

            var ingredients = _ingredientService.GetIngredientsForItemOnlyName(id, restaurantId);
            var ingredientViewModel = ingredients
                .Select(IngredientViewModel.ConvertToViewModel)
                .ToList();

            var compositeViewModel = new CompositeViewModelMenuItemNutritionAllergen
            {
                MenuItemViewModel = menuItemViewModel,
                NutritionViewModel = nutritionViewModels,
                AllergenViewModel = allergenViewModel,
                IngredientViewModel = ingredientViewModel
            };

            ViewBag.Category = category;
            return View("Info", compositeViewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            _cartService.AddToCart(id, quantity);
            ViewBag.CartCount = _cartService.GetCartCount();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult DismissMessage()
        {
            TempData["Anti-Abuse"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}