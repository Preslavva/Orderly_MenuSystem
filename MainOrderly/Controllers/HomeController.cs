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

        public HomeController(ILogger<HomeController> logger, CartService cartService, MenuService menuService, NutritionService nutritionService)
        {
            _logger = logger;
            _cartService = cartService;
            _menuService = menuService;
            _nutritionService = nutritionService;

        }

        //this controller is the first thing that user you will see.
        public IActionResult LoadingPage(int tableId)
        {
            ViewBag.TableId = tableId;  
            return View("~/Views/loading.cshtml");
        }

        [HttpGet]
        public IActionResult Index(string searchTerm = "", int tableId = 0)
        {
            if (tableId > 0)
            {
                HttpContext.Session.SetInt32("TableId", tableId);
            }
            List<MenuItem>? menu = _menuService.LoadMenuItems(); //from the dto i will create viewModel


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
        public IActionResult Search(string term, Category? category)
        {
            var allItems = _menuService.LoadMenuItems();
            var menuItemViewModel = MappingHelper.ConvertToViewModels(allItems);

            var filtered = menuItemViewModel.Where(x =>
                (string.IsNullOrWhiteSpace(term) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(term, StringComparison.OrdinalIgnoreCase))) &&
                (!category.HasValue || x.Category == category.Value)
            ).ToList();

            return PartialView("_MenuItemList", filtered);
        }


    }
}
