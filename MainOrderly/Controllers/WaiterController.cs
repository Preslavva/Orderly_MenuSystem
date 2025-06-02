using Microsoft.AspNetCore.Mvc;
using Services;
using MainOrderly.WebApp.ViewModels;
using MainOrderly.WebApp.Attributes;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Waiter")]
    public class WaiterController : Controller
    {
        private readonly WaiterService _waiterService;
        private readonly KitchenOrderService _orderService;
        private readonly TableService _tableService;

        public WaiterController(WaiterService waiterService, KitchenOrderService orderService, TableService tableService)
        {
            _waiterService = waiterService;
            _orderService = orderService;
            _tableService = tableService;
        }

        // Main page that renders the container
        public IActionResult Index()
        {
            var orders = _waiterService.GetCompletedOrdersWithItems();
            var viewModel = orders.Select(OrderViewModel.ConvertToViewModel).ToList();
            ViewBag.IsWaiter = true;

            return View("WaiterTab", viewModel); // Return the main view
        }

        // HTMX endpoint that returns only the partial content
        public IActionResult Orders()
        {
            var orders = _waiterService.GetCompletedOrdersWithItems();
            var viewModel = orders.Select(OrderViewModel.ConvertToViewModel).ToList();
            ViewBag.IsWaiter = true;

            return PartialView("_WaiterOrdersPartial", viewModel);
        }

        [HttpPost]
        public IActionResult DeliverOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var tableId = order.Table.Id;
            var tableNumber = _tableService.GetTableNumberById(tableId);

            _waiterService.UpdateOrderStatusDelivered(orderId);
            TempData["Message"] = $"Order {order.Id} to table {tableNumber} was successfully delivered";

            // For HTMX requests, return the updated partial view
            if (Request.Headers["HX-Request"].Any())
            {
                var orders = _waiterService.GetCompletedOrdersWithItems();
                var viewModel = orders.Select(OrderViewModel.ConvertToViewModel).ToList();
                return PartialView("_WaiterOrdersPartial", viewModel);
            }

            return RedirectToAction("Index");
        }
    }
}