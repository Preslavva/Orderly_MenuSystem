using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using MainOrderly.WebApp.Extensions;

namespace MainOrderly.WebApp.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public IActionResult History()
        {
            ViewData["Page"] = "History";
            var user = HttpContext.Session.GetAuthenticatedUser();
            int restaurantId = user?.RestaurantId ?? 1;
            
            List<OrderHistory> orders = _historyService.GetHistory(restaurantId);
            List<OrderHistoryViewModel> historyViewModel = orders.Select(OrderHistoryViewModel.ConvertToViewModel).ToList();
            return View(historyViewModel);
        }
    }
}