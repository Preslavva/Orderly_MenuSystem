using Microsoft.AspNetCore.Mvc;

namespace MainOrderly.WebApp.Controllers
{
    public class BusinessController : Controller
    {
        
        public IActionResult Index()
        {
            return View("LandingPage");
        }
        
        public IActionResult LandingPage()
        {
            return View();
        }

        public IActionResult Analytics()
        {
            return View();
        }
    }
}