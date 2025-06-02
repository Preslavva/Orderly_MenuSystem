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
        private readonly RestaurantService _restaurantService;

        public HistoryController(HistoryService historyService, RestaurantService restaurantService) : base(restaurantService)
        {
            _historyService = historyService;
            _restaurantService = restaurantService;
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