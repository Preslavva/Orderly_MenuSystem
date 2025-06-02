using Microsoft.AspNetCore.Mvc;
using Services;
using MainOrderly.WebApp.ViewModels;

namespace MainOrderly.WebApp.Controllers
{
    public class WaiterController : Controller
    {
        private readonly WaiterService _waiterService;

        public WaiterController(WaiterService waiterService)
        {
            _waiterService = waiterService;
        }

        public IActionResult WaiterTab()
        {
            var orders = _waiterService.GetCompletedOrdersWithItems();
            var viewModel = orders.Select(OrderViewModel.ConvertToViewModel).ToList();
            return View(viewModel);
        }

        public IActionResult DeliverOrder(int orderId)
        {
            _waiterService.UpdateOrderStatusDelivered(orderId);
            return RedirectToAction("WaiterTab");
        }
    }
}
