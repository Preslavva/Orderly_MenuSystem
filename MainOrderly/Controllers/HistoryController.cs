using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using MainOrderly.WebApp.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MainOrderly.WebApp.Controllers
{
    public class HistoryController(HistoryService historyService, RestaurantService restaurantService)
        : BaseController(restaurantService)
    {
        [HttpGet]
        public IActionResult History()
        {
            ViewData["Page"] = "History";
            var restaurantId = HttpContext.Session.GetInt32("RestaurantId");
            ApplyStyling((int)restaurantId);
            List<OrderHistory> orders = historyService.GetHistory((int)restaurantId);
            List<OrderHistoryViewModel> historyViewModel = orders.Select(OrderHistoryViewModel.ConvertToViewModel).ToList();
            return View(historyViewModel);
        }
    }
}