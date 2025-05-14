using System.Diagnostics;
using System.Xml.Linq;
using MainOrderly.WebApp.Helpers;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using MSSQL;
using Services;

namespace MainOrderly.WebApp.Controllers
{//[Route("register")] for example in the url, something in the future
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
        private int restaurantId = 1;

        public HomeController(ILogger<HomeController> logger, CartService cartService, MenuService menuService, NutritionService nutritionService, AllergenService allergenService, IngredientService ingredientService, TableService tableService)
        {
            _logger = logger;
            _cartService = cartService;
            _menuService = menuService;
            _tableService = tableService;
            _nutritionService = nutritionService;
            _allergenService = allergenService;
            _ingredientService = ingredientService;

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
        public IActionResult Index(Category? category = null,string token="", string searchTerm = "", int restaurantId = 1)
        {
            ViewData["Page"] = "Index";
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

            List<MenuItem>? menu = _menuService.LoadMenuItems(restaurantId);

            var allMenuItems = _menuService.LoadMenuItems(restaurantId);
        

     
    

            //foreach (var menuItem in allMenuItems)
            //{
            //    menuItem.Ingredients.ForEach(ingredient =>
            //    {
            //        var ingredientToUpdate = _ingredientService.GetIngredientById(ingredient.IngredientId);
            //        if (ingredientToUpdate.QuantityInStock < ingredient.Quantity)
            //        {
            //            menuItem.SetMenuItemAvailability(false);
            //        }
            //    });
            //}



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

            var ingredients = _ingredientService.GetIngredientsForItemOnlyName(id);
            
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
