using Microsoft.AspNetCore.Mvc;

namespace MainOrderly.WebApp.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
