using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;
using MainOrderly.WebApp.Helpers;
using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace MainOrderly.WebApp.Controllers
{
    public class KitchenController : Controller
    {
        private readonly KitchenOrderService _kitchenOrderService;
        private readonly TimerHelpers _timerHelpers;

        public KitchenController(KitchenOrderService kitchenOrderService, TimerHelpers timerHelpers)
        {
            _kitchenOrderService = kitchenOrderService;
            _timerHelpers = timerHelpers;
        }

        private int GetRestaurantId()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            return user?.RestaurantId ?? 1;
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult Dashboard(int id = 0)
        {
            int restaurantId = GetRestaurantId();
            
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER, restaurantId));
            kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING, restaurantId));
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED, restaurantId));

            var viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));

            

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
            int restaurantId = GetRestaurantId();
            
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetNewOrders(_kitchenOrderService.GetOrderByStatus(OrderStatus.NEW_ORDER, restaurantId));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                
            KitchenViewModelWithTimer kitchenViewModelWithTimer = new KitchenViewModelWithTimer
            {
                CompositeKitchenViewModel = viewModel,
                TimerViewModel = new TimerViewModel()
            };

            return PartialView("_NewOrdersPartial", kitchenViewModelWithTimer);
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult GetProcessingOrders()
        {
            int restaurantId = GetRestaurantId();
            
            var km = new KitchenOrderManager();
            km.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING, restaurantId));

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
            int restaurantId = GetRestaurantId();
            
            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED, restaurantId));

            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), 
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                
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
            int restaurantId = GetRestaurantId();
            _kitchenOrderService.UpdateOrderItemStatus(orderId, menuItemId, newStatus, restaurantId);

            var manager = new KitchenOrderManager();
            manager.SetPendingOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.PROCESSING, restaurantId));
            manager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED, restaurantId));

            var menuItems = _kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING, orderId,restaurantId);
            var orderPartialComplete = menuItems.Any(m => m.Status == OrderStatus.PROCESSING);

            if (!orderPartialComplete)
                _kitchenOrderService.UpdateOrderStatus(orderId, OrderStatus.COMPLETED, restaurantId);

            var vm = CompositeKitchenViewModel.ConvertToViewModel(manager, 
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                
            return PartialView("_ProcessingOrdersPartial", new KitchenViewModelWithTimer { CompositeKitchenViewModel = vm, TimerViewModel = new() });
        }

        [HttpPost]
        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            int restaurantId = GetRestaurantId();
            
            if (newStatus == OrderStatus.PROCESSING)
            {
                _timerHelpers.RecordStartTime(id);
            }

            if (newStatus == OrderStatus.COMPLETED)
            {
                _timerHelpers.RemoveTimer(id);
            }
            _kitchenOrderService.UpdateOrderStatus(id, newStatus, restaurantId);

            if (newStatus == OrderStatus.PROCESSING)
            {
                var orders = _kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING, id,restaurantId);
                foreach (var menu in orders) 
                    _kitchenOrderService.UpdateOrderItemStatus(id, menu.MenuItemId, OrderStatus.PROCESSING, restaurantId);

                KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
                kitchenOrderManager.SetPendingOrders(_kitchenOrderService.GetMenuItemsOrder(OrderStatus.PROCESSING, id,restaurantId));

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                    (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId),
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
                kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED, restaurantId));

                CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                    (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), 
                    orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                    
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

            OrderViewModel order = OrderViewModel.ConvertToViewModel(_kitchenOrderService.GetOrderById(orderId, 1));
            return Content(order.Status.ToString());
        }

        [RequireRole("Owner", "Manager", "Chef")]
        public IActionResult RemoveOrderDashboard(List<int> id)
        {
            int restaurantId = GetRestaurantId();
            
            _kitchenOrderService.RemoveOrderFromDashboard(id,restaurantId);

            KitchenOrderManager kitchenOrderManager = new KitchenOrderManager();
            kitchenOrderManager.SetCompletedOrders(_kitchenOrderService.GetMenuItemsByStatus(OrderStatus.COMPLETED, restaurantId));
            
            CompositeKitchenViewModel viewModel = CompositeKitchenViewModel.ConvertToViewModel(kitchenOrderManager, 
                (orderId, _) => _timerHelpers.HasExceededPrepTime(orderId), 
                orderId => _timerHelpers.GetRemainingTime(orderId, out _));
                
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
