      
using System.Net;               
using System.Text.RegularExpressions;
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
            Restaurant restaurant = _restaurantService.GetRestaurantById(1);

            ViewBag.ColorDefault = restaurant.ColorDefault ?? "#000000";
            ViewBag.ColorButtons = restaurant.ColorButtons ?? "#4CAF50";
            ViewBag.ColorBackground = restaurant.ColorBackground ?? "#ffffff";

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

            base.OnActionExecuting(context);
        }
    }

    public static class SafeGoogleFont
    {
        private static readonly Regex _ok =
            new(@"^[A-Za-z0-9\- ]{1,64}$", RegexOptions.Compiled);

        public static string? ToGoogleParam(string? dbValue)
        {
            if (string.IsNullOrWhiteSpace(dbValue))
                return null;

            string trimmed = dbValue.Trim();

            if (!_ok.IsMatch(trimmed))
                return null;                  

            return WebUtility.UrlEncode(trimmed)
                             .Replace("%20", "+");
        }
    }
}
