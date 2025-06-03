using MainOrderly.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class BusinessController(AnalyticsService analyticsService) : Controller
    {
        private readonly AnalyticsService _analyticsService = analyticsService;

        private int GetRestaurantId()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            return user?.RestaurantId ?? 1;
        }
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


        public JsonResult GetTopSellingItems(string date)
        {
            var restaurantId = GetRestaurantId();
            
            var actualDate = DateTime.TryParse(date, out var parsedDate) ? parsedDate : DateTime.MinValue;
            if (actualDate == DateTime.MinValue) actualDate = DateTime.Today;
            var year = actualDate.Year;
            var selectedMonth = (int)actualDate.Month;
            var topSellingItems = _analyticsService.GetTopSellingItems(year, selectedMonth, restaurantId);
            var labels = topSellingItems.Select(i => i.MenuItemName).ToList();
            var data = topSellingItems.Select(i => i.QuantitySold).ToList();
            return Json(new { labels, data });
        }
        public JsonResult GetRevenueByDataRange(string date)
        {
            var restaurantId = GetRestaurantId();
            var actualDate = DateTime.TryParse(date, out var parsedDate) ? parsedDate : DateTime.MinValue;

            if (actualDate == DateTime.MinValue) actualDate = DateTime.Today;

            var year = actualDate.Year;
            var selectedMonth = (int)actualDate.Month;

            var entries = _analyticsService.GetRevenueByDataRange(year, selectedMonth, restaurantId);

            var labels = entries.Select(g => g.Date.ToString("dd")).ToList(); // OR "MM-dd"
            var data = entries.Select(g => g.Revenue).ToList();

            return Json(new { labels, data });
        }

        public JsonResult GetRevenueByCategory(string date)
        {
            var restaurantId = GetRestaurantId();
            var actualDate = DateTime.TryParse(date, out var parsedDate) ? parsedDate : DateTime.MinValue;

            if (actualDate == DateTime.MinValue) actualDate = DateTime.Today;

            var year = actualDate.Year;
            var selectedMonth = (int)actualDate.Month;

            var entries = _analyticsService.GetCategoryRevenues(year, selectedMonth,restaurantId);

            var labels = entries.Select(e => e.CategoryName);
            var data = entries.Select(e => e.TotalRevenue);

            return Json(new { labels, data });


        }

        public JsonResult GetHourlyOrders(string date)
        {
            var restaurantId = GetRestaurantId();
            var actualDate = DateTime.TryParse(date, out var parsedDate) ? parsedDate : DateTime.MinValue;

            if (actualDate == DateTime.MinValue) actualDate = DateTime.Today;

            var year = actualDate.Year;
            var selectedMonth = (int)actualDate.Month;


            var hourlyOrders = _analyticsService.GetHourlyOrders(restaurantId,selectedMonth,year);
            var labels = hourlyOrders.Select(h => h.Hour);
            var data = hourlyOrders.Select(o => o.OrderCount).ToList();
            return Json(new { labels, data });
        }
    }
}