using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;

namespace MainOrderly.WebApp.Controllers
{
    public class HistoryController : Controller
    {
        private readonly HistoryService _historyService;
        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public IActionResult History()
        {
            List<OrderHistory> orders = _historyService.GetHistory();
            List<OrderHistoryViewModel> historyViewModel = orders.Select(OrderHistoryViewModel.ConvertToViewModel).ToList();
            return View(historyViewModel);
        }
    }
}
