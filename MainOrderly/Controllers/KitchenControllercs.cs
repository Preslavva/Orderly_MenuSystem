using Microsoft.AspNetCore.Mvc;

using Models.Entities;
using Models.Enums;
using Services;


namespace MainOrderly.WebApp.Controllers
{
    public class KitchenController : Controller
    {
        private KitchenOrderService _kitchenOrderService;
        public KitchenController(KitchenOrderService kitchenOrderService)
        {
            _kitchenOrderService = kitchenOrderService;
        }

        public IActionResult Dashboard()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager()
            {
                NewOrders = _kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER),
                PendingOrders = _kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING),
                CompletedOrders = _kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED),
            };
            return View("~/Views/Kitchen/Dashboard.cshtml", kitchenOrderManager);
        }


        public IActionResult UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            _kitchenOrderService.UpdateOrderStatus(id, newStatus);
            return RedirectToAction("Dashboard");
        }

        public IActionResult RemoveOrderDashboard(int id)
        {
            _kitchenOrderService.RemoveOrderFromDashboard(id);
            return RedirectToAction("Dashboard");
        }

    }
}