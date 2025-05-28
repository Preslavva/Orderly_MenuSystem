using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;

namespace MainOrderly.WebApp.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly HistoryService _historyService;
        public HistoryController(RestaurantService restaurantService, HistoryService historyService) :base(restaurantService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public IActionResult History()
        {
            ViewData["Page"] = "History";
            List<OrderHistory> orders = _historyService.GetHistory();
            List<OrderHistoryViewModel> historyViewModel = orders.Select(OrderHistoryViewModel.ConvertToViewModel).ToList();
            return View(historyViewModel);
        }
    }
}
