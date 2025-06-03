      
using System.Net;               
using System.Text.RegularExpressions;
using System.Xml;
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
