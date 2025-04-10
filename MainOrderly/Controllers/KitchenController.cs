using MainOrderly.WebApp.ViewModels;
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
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();

            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
                                   
            return View("~/Views/Kitchen/Dashboard.cshtml", viewModel);
        }


        public IActionResult UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            _kitchenOrderService.UpdateOrderStatus(id, newStatus);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult GetOrderStatus(int orderId)
        {
            OrderViewModel order = OrderViewModel.ConvertToViewModel(_kitchenOrderService.GetOrderById(orderId));
            return Content(order.Status.ToString());
            //return PartialView("orderButtons",order);
        }

        public IActionResult RemoveOrderDashboard(int id)
        {
            _kitchenOrderService.RemoveOrderFromDashboard(id);
            return RedirectToAction("Dashboard");
        }

    }
}