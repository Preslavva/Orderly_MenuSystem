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
            Models.Entities.MenuItem? menuItem = _menuService.GetMenuItem(id);

            ViewBag.Nutritions = _nutritionService.GetNutritionForMenuItem(id);

            return View("Info", menuItem);
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
    }
}
