using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;
using MainOrderly.WebApp.Helpers;
using System.Collections.Generic;
using MainOrderly.WebApp.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace MainOrderly.WebApp.Controllers
{
    public class KitchenController : Controller
    {
        private KitchenOrderService _kitchenOrderService;
        private TimerHelpers _timerHelpers;

        public KitchenController(KitchenOrderService kitchenOrderService, TimerHelpers timerHelpers)
        {
            _kitchenOrderService = kitchenOrderService;
            _timerHelpers = timerHelpers;
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult Dashboard(int id)
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING));
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED));

            var viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager,(orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId,out _));

            

            var vmWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };
         
            return View(vmWithTimer);
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult GetNewOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_NewOrdersPartial", kitchenViewModelWithTimer);
        }

        //public IActionResult GetProcessingOrders()
        //{
        //    KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
        //    kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));

        //    CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
        //    if (viewModel.PendingOrders.Count != 0)
        //    {
        //        foreach (var order in viewModel.PendingOrders)
        //        {
        //            order.ElapsedTime = _timerHelpers.GetElapsedTime(order.Id);
        //            order.IsExceeded = _timerHelpers.HasExceededPrepTime(order.Id);
        //        }
        //    }

        //    KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
        //    {
        //        CompositeKitchenViewModel = viewModel,
        //        TimerViewModel = new TimerViewModel()
        //    };

        //    return PartialView("_ProcessingOrdersPartial", kitchenViewModelWithTimer);
        //}

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult GetProcessingOrders()
        {
            var km = new KitchenOrderManager();
            km.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING));

            var viewModel = CompositeKitchenViewModel.ConvertToViewModel(
                km,
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));

            var wrapper = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };
            return PartialView("_ProcessingOrdersPartial", wrapper);
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult GetCompletedOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), orderId => _timerHelpers.GetRemainingTime(orderId, out _));
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_CompletedOrdersPartial", kitchenViewModelWithTimer);
        }


        [HttpPost]
        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult UpdateOrderItemStatus(int orderId, int menuItemId, OrderStatus newStatus)
        {
            _kitchenOrderService.UpdateOrderItemStatus(orderId, menuItemId, newStatus);

            var manager = new KitchenOrderManager();
            manager.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING));
            manager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED));

            var menuItems = _kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING,orderId);

            var orderPartialComplete = menuItems.Any(m => m.Status == OrderStatus.PROCESSING);

            if(!orderPartialComplete)
                _kitchenOrderService.UpdateOrderStatus(orderId,OrderStatus.COMPLETED);

            var vm = CompositeKitchenViewModel.ConvertToViewModel(manager,(orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
            return PartialView("_ProcessingOrdersPartial", new KitchenViewModelWithTimer { CompositeKitchenViewModel = vm, TimerViewModel = new() });
        }



        [HttpPost]
        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            if (newStatus == OrderStatus.PROCESSING)
            {
                _timerHelpers.RecordStartTime(id);
            }

            if (newStatus == OrderStatus.COMPLETED)
            {
                _timerHelpers.RemoveTimer(id);
            }

            _kitchenOrderService.UpdateOrderStatus(id, newStatus);
            

            if (newStatus == OrderStatus.PROCESSING)
            {
                var orders = _kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING,id);
                foreach (var menu in orders) 
                    _kitchenOrderService.UpdateOrderItemStatus(id, menu.MenuItemId, OrderStatus.PROCESSING);

                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING, id));

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                    orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
                {
                    CompositeKitchenViewModel = viewModel,
                    TimerViewModel = new TimerViewModel()
                };

                return PartialView("_ProcessingOrdersPartial", kitchenViewModelWithTimer);
            }
            if (newStatus == OrderStatus.COMPLETED)
            {
                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED));

            

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
                {
                    CompositeKitchenViewModel = viewModel,
                    TimerViewModel = new TimerViewModel()
                };

                return PartialView("_CompletedOrdersPartial", kitchenViewModelWithTimer);
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetOrderStatus(int orderId)
        {
            if (orderId == 0)
                return BadRequest("Order ID is required.");

            OrderViewModel order = OrderViewModel.ConvertToViewModel(_kitchenOrderService.GetOrderById(orderId));
            return Content(order.Status.ToString());
        }

        [RequireRole("Owner", "Manager", "Chef")]

        public IActionResult RemoveOrderDashboard(List<int> id)
        {
            _kitchenOrderService.RemoveOrderFromDashboard(id);

            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED));
            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), orderId => _timerHelpers.GetRemainingTime(orderId, out _));
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_CompletedOrdersPartial", kitchenViewModelWithTimer);
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "BusinessAccount");
        }
    }
}
