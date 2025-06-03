using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly RestaurantService _restaurantService;

        public BaseController(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
           Restaurant restaurant = _restaurantService.GetRestaurantById(20);

            ViewBag.ColorDefault = restaurant.ColorDefault ?? "#000000";
            ViewBag.ColorButtons = restaurant.ColorButtons ?? "#4CAF50";
            ViewBag.ColorBackground = restaurant.ColorBackground ?? "#ffffff";

            var result = (restaurant.Font ?? string.Empty)
     .Split(',', StringSplitOptions.RemoveEmptyEntries)
     .Select(s => s.Trim())
     .ToList();

            ViewBag.FontName = result.ElementAtOrDefault(0) ?? "Arial";
            ViewBag.FontFamily = result.ElementAtOrDefault(1) ?? "sans-serif";


            if (restaurant.Logo != null)
            {
                string base64Logo = Convert.ToBase64String(restaurant.Logo);
                ViewBag.LogoDataUrl = $"data:image/png;base64,{base64Logo}";
            }
            else
            {
                ViewBag.LogoDataUrl = "/images/default-logo.png";
            }

            base.OnActionExecuting(context);
        }
    }
}

