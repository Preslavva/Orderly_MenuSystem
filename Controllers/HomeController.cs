using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderlyTest.Models;
using OrderlyTest.repos;
using OrderlyTest.services;

namespace OrderlyTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartServices _cartServices;
        private readonly MenuItemDB _menuDb;

        public HomeController(ILogger<HomeController> logger, CartServices cartServices, MenuItemDB menuDb)
        {
            _logger = logger;
            this._cartServices = cartServices;
            this._menuDb = menuDb;
        }

        [HttpGet]
        public IActionResult Index()
        {/*
           //List<MenuItem> menu = new List<MenuItem>();
            List<MenuItem> menu = new List<MenuItem>()
            {
                new MenuItem("burger", "some burger", 2.5m,true, "image")
            };
            ViewBag.CartCount = _cartServices.GetCartCount();
            return View(menu);
            */
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart()
        {
            _cartServices.AddToCart();   
            return RedirectToAction("Index","Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
