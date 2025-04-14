using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using MainOrderly.WebApp.Helpers;
using Models.Enums;

namespace MainOrderly.WebApp.Controllers
{//[Route("register")] for example in the url, something in the future
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartService _cartService;
        private readonly MenuService _menuService;
        private readonly NutritionService _nutritionService;
        private readonly AllergenService _allergenService;

        private static Dictionary<int, string> _tableGuidMap = new();
        private int restaurantId = 1;

        public HomeController(ILogger<HomeController> logger, CartService cartService, MenuService menuService, NutritionService nutritionService, AllergenService allergenService)
        {
            _logger = logger;
            _cartService = cartService;
            _menuService = menuService;
            _nutritionService = nutritionService;
            _allergenService = allergenService;

        }

        //this controller is the first thing that user you will see.
        public IActionResult LoadingPage(int tableId)
        {
            ViewBag.TableId = tableId;  
            return View("~/Views/loading.cshtml");
        }

        [HttpGet]
        public IActionResult Index(Category? category = null, string searchTerm = "", int tableId = 0, int restaurantId = 0)
        {
            if (tableId > 0)
            {
                HttpContext.Session.SetInt32("TableId", tableId);

                if (!_tableGuidMap.ContainsKey(tableId))
                {
                    _tableGuidMap[tableId] = Guid.NewGuid().ToString();
                }

                string tableGuid = _tableGuidMap[tableId];

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

            if(!Request.Cookies.ContainsKey("SessiondID"))
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

            List<MenuItem>? menu = _menuService.LoadMenuItems(restaurantId);

            var allMenuItems = _menuService.LoadMenuItems(restaurantId);

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
            var menuItem = _menuService.GetMenuItem(id, restaurantId);
            var menuItemViewModel = MenuItemViewModel.ConvertToViewModel(menuItem);
        

            MenuItemViewModel menItemViewModel = MenuItemViewModel.ConvertToViewModel(menuItem); // then we convert to entities to a viewmodel

            var nutritions = _nutritionService.GetNutritionForMenuItem(id);
            var nutritionViewModels = nutritions
                .Select(NutritionViewModel.ConvertToViewModel)
                .ToList();

            var allergens = _allergenService.GetAllergenForMenuItem(id);
            var allergenViewModel = allergens
                .Select(AllergenViewModel.ConvertToViewModel)
                .ToList();

            var compositeViewModel = new CompositeViewModelMenuItemNutritionAllergen
            {
                MenuItemViewModel = menuItemViewModel,
                NutritionViewModel = nutritionViewModels,
                AllergenViewModel = allergenViewModel
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
        //[HttpGet]
        //public IActionResult Search(string term, Category? category)
        //{
        //    var allItems = _menuService.LoadMenuItems();
        //    var menuItemViewModel = MappingHelper.ConvertToViewModels(allItems);
        //    int restaurantId = HttpContext.Session.GetInt32("RestaurantId") ?? 0;
        //    var allItems = _menuService.LoadMenuItems(restaurantId);

        //    List<MenuItemViewModel> menuItemViewModel = MappingHelper.ConvertToViewModels(allItems);

        //    var filtered = menuItemViewModel.Where(x =>
        //        (string.IsNullOrWhiteSpace(term) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(term, StringComparison.OrdinalIgnoreCase))) &&
        //        (!category.HasValue || x.Category == category.Value)
        //    ).ToList();

        //    return PartialView("_MenuItemList", filtered);
        //}


    }
}
