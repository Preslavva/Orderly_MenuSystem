using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;

namespace MainOrderly.WebApp.Controllers
{//[Route("register")] for example in the url, something in the future
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartServices _cartServices;
        private readonly MenuService _menuService;
        private readonly NutritionService _nutritionService;

        public HomeController(ILogger<HomeController> logger, CartServices cartServices, MenuService menuService, NutritionService nutritionService)
        {
            _logger = logger;
            _cartServices = cartServices;
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
            List<MenuItem>? menu = _menuService.LoadMenuItems();


            if (!string.IsNullOrEmpty(searchTerm))
            {
                menu = menu.Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                           .ToList();
            }

            TempData["CartCount"] = _cartServices.GetCartCount();

            return View(menu);
        }

        [HttpGet]
        public IActionResult GetItemInfo(int id)
        {
            MenuItem? menuItem = _menuService.GetMenuItem(id);

            ViewBag.Nutritions = _nutritionService.GetNutritionForMenuItem(id);

            return View("Info", menuItem);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            _cartServices.AddToCart(id, quantity);
            ViewBag.CartCount = _cartServices.GetCartCount();
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
            var allItems = _menuService.LoadMenuItems();

            var filtered = string.IsNullOrWhiteSpace(term)
                ? allItems
                : allItems.Where(x =>
                    !string.IsNullOrEmpty(x.Name) &&
                    x.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                ).ToList();

            return PartialView("_MenuItemList", filtered);
        }


    }
}
