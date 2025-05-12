using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using MSSQL;
using Services;
using MainOrderly.WebApp.Helpers;
using System.Collections.Generic;
using MainOrderly.WebApp.Attributes;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner", "Manager", "Chef")]
    public class KitchenController : Controller
    {
        private KitchenOrderService _kitchenOrderService;
        private TimerHelpers _timerHelpers;

        public KitchenController(KitchenOrderService kitchenOrderService, TimerHelpers timerHelpers)
        {
            _kitchenOrderService = kitchenOrderService;
            _timerHelpers = timerHelpers;
        }

        public IActionResult Dashboard()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));

            if (kitchenOrderManager.PendingOrders.Count != 0)
            {
                foreach (var order in kitchenOrderManager.PendingOrders)
                {
                    _timerHelpers.GetElapsedTime(order.Id);
                }
            }

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return View(kitchenViewModelWithTimer);
        }

        public IActionResult GetNewOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_NewOrdersPartial", kitchenViewModelWithTimer);
        }

        public IActionResult GetProcessingOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
            if (viewModel.PendingOrders.Count != 0)
            {
                foreach (var order in viewModel.PendingOrders)
                {
                    order.ElapsedTime = _timerHelpers.GetElapsedTime(order.Id);
                    order.IsExceeded = _timerHelpers.HasExceededPrepTime(order.Id);
                }
            }

            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_ProcessingOrdersPartial", kitchenViewModelWithTimer);
        }

        public IActionResult GetCompletedOrders()
        {
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_CompletedOrdersPartial", kitchenViewModelWithTimer);
        }

        [HttpPost]
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
                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.PROCESSING));

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
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
                kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
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
        public IActionResult GetOrderStatus(int orderId)
        {
            OrderViewModel order = OrderViewModel.ConvertToViewModel(_kitchenOrderService.GetOrderById(orderId));
            return Content(order.Status.ToString());
            //return PartialView("orderButtons",order);
        }

        public IActionResult RemoveOrderDashboard(List<int> id)
        {
            _kitchenOrderService.RemoveOrderFromDashboard(id);

            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.COMPLETED));
            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager);
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_CompletedOrdersPartial", kitchenViewModelWithTimer);
        }
        
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "BusinessAccount");
        }
    }
}
