using System.Xml;
using MainOrderly.WebApp.Helpers;
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

        public void ApplyStyling(int restId)
        {
            Restaurant restaurant = _restaurantService.GetRestaurantById(restId);

            ViewBag.ColorDefault = restaurant.ColorDefault;
            ViewBag.ColorButtons = restaurant.ColorButtons;
            ViewBag.ColorBackground = restaurant.ColorBackground;
            var parts = (restaurant.Font ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            string fontName = parts.ElementAtOrDefault(0) ?? "Arial";
            string genericFamily = parts.ElementAtOrDefault(1) ?? "sans-serif";

            ViewBag.FontName = fontName;
            ViewBag.FontFamily = genericFamily;

            string? familyParam = SafeGoogleFont.ToGoogleParam(fontName);

            ViewBag.GoogleFontLink = familyParam is null
                ? null
                : $"https://fonts.googleapis.com/css2?family={familyParam}&display=swap";

            ViewBag.LogoDataUrl = restaurant.Logo is { Length: > 0 }
                ? $"data:image/png;base64,{Convert.ToBase64String(restaurant.Logo)}"
                : "/images/default-logo.png";

        }

    }
}
