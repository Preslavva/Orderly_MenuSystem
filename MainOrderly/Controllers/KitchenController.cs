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

            return View(viewModel);
        }

        public IActionResult GetUpdatedOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));
            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);

            return PartialView("_OrderBoardPartial", viewModel);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            _kitchenOrderService.UpdateOrderStatus(id, newStatus);

            // Check if the request is coming from HTMX
            if (Request.Headers.TryGetValue("HX-Request", out var headerValue) && headerValue == "true")
            {
                // Return the updated partial view for HTMX requests
                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
                kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));
                kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));
                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);

                return PartialView("_OrderBoardPartial", viewModel);
            }

       
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult RemoveOrderDashboard(int id)
        {
            _kitchenOrderService.RemoveOrderFromDashboard(id);

            // Check if the request is coming from HTMX
            if (Request.Headers.TryGetValue("HX-Request", out var headerValue) && headerValue == "true")
            {
    
                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
                kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));
                kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));
                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);

                return PartialView("_OrderBoardPartial", viewModel);
            }

    
            return RedirectToAction("Dashboard");
        }
        }
    }
