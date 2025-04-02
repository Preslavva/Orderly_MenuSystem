using Microsoft.AspNetCore.Mvc;

namespace MainOrderly.WebApp.Controllers
{
    public class BusinessController : Controller
    {
        
        public IActionResult LandingPage()
        {
            return View("~/Views/Business/LandingPage.cshtml");  
        }
    }
}