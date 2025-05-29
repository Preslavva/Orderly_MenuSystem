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
            Restaurant restaurant = _restaurantService.GetRestaurantById(11);

            ViewBag.ColorDefault = restaurant.ColorDefault ?? "#000000";
            ViewBag.ColorButtons = restaurant.ColorButtons ?? "#4CAF50";
            ViewBag.ColorBackground = restaurant.ColorBackground ?? "#ffffff";

            ViewBag.Font = restaurant.Font ?? "Arial";

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

