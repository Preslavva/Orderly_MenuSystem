using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using MainOrderly.WebApp.Helpers;

namespace MainOrderly.WebApp.Controllers
{//[Route("register")] for example in the url, something in the future
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartService _cartService;
        private readonly MenuService _menuService;
        private readonly NutritionService _nutritionService;
        private readonly TableService _tableService;
        private static Dictionary<int, string> _tableGuidMap = new();

        public HomeController(ILogger<HomeController> logger, CartService cartService, MenuService menuService, TableService tableService, NutritionService nutritionService)
        {
            _logger = logger;
            _cartService = cartService;
            _menuService = menuService;
            _tableService = tableService;
            _nutritionService = nutritionService;

        }

        //this controller is the first thing that user you will see.
        public IActionResult LoadingPage(string token)
        {
            Table table = _tableService.GetTableByToken(token);

            if (table == null)
            {
                return NotFound("Invalid token.");
            }

            HttpContext.Session.SetInt32("TableId", table.Id);
  
            return View("~/Views/loading.cshtml");
        }

        [HttpGet]
        public IActionResult Index(string token="", string searchTerm = "", int restaurantId = 0)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var table = _tableService.GetTableByToken(token);
                if (table != null)
                {
                    HttpContext.Session.SetInt32("TableId", table.Id);
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


            if (!string.IsNullOrEmpty(searchTerm))
            {
                menu = menu.Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                           .ToList();
            }

            List<MenuItemViewModel> menuItemViewModel =  MappingHelper.ConvertToViewModels(menu);
            TempData["CartCount"] = _cartService.GetCartCount();

            return View(menuItemViewModel);
        }


        [HttpGet]
        public IActionResult GetItemInfo(int id)
        {
            MenuItem? menuItem = _menuService.GetMenuItem(id); // get first the object as entities

            MenuItemViewModel menItemViewModel = MenuItemViewModel.ConvertToViewModel(menuItem); // then we convert to entities to a viewmodel

            List<Nutrition> nutritions = _nutritionService.GetNutritionForMenuItem(id); // same goes for nutritions.

            List<NutritionViewModel> nutritionViewModels = nutritions.Select(nutrition =>NutritionViewModel.ConvertToViewModel(nutrition)).ToList();

            CompositeViewModelMenuItemNutrition compositeViewModel = new CompositeViewModelMenuItemNutrition
            {
                MenuItemViewModel = menItemViewModel,
                NutritionViewModel = nutritionViewModels
            };

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

        [HttpGet]
        public IActionResult Search(string term)
        {
            int restaurantId = HttpContext.Session.GetInt32("RestaurantId") ?? 0;
            var allItems = _menuService.LoadMenuItems(restaurantId);

            List<MenuItemViewModel> menuItemViewModel = MappingHelper.ConvertToViewModels(allItems);

            var filtered = string.IsNullOrWhiteSpace(term)? menuItemViewModel: menuItemViewModel.Where(x =>!string.IsNullOrEmpty(x.Name) && x.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

            return PartialView("_MenuItemList", filtered);
        }


    }
}
