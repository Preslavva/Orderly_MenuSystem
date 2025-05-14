using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class CompositeKitchenViewModel
    {
        public List<MenuItemViewModel> MenuItems { get; set; }
        public List<OrderViewModel> NewOrders { get; set; }   // whole orders
        public List<OrderLineItemViewModel> PendingOrders { get; set; }   // single items
        public List<OrderLineItemViewModel> CompletedOrders { get; set; }   // single items

        public CompositeKitchenViewModel()
        {
            MenuItems = new();
            NewOrders = new();
            PendingOrders = new();
            CompletedOrders = new();
        }

        public static CompositeKitchenViewModel ConvertToViewModel(KitchenOrderManager km, Func<int, int,bool> exceededFn, Func<int, string> elapsedFn)      
        {
            var newOrders = km.NewOrders
                              .Select(OrderViewModel.ConvertToViewModel)
                              .ToList();

           
            var pendingItems = km.PendingOrders
                                 .Select(oi => OrderLineItemViewModel.FromOrderItem(oi,oi.MenuItem.RestaurantId, elapsedFn(oi.OrderId),
                                    exceededFn(oi.OrderId, oi.MenuItem.PrepTime)))
                                 .ToList();

            var completedItems = km.CompletedOrders
                                    .Select(oi => OrderLineItemViewModel.FromOrderItem(
                                                       oi,
                                                       oi.MenuItem.RestaurantId,
                                                       elapsedFn(oi.OrderId),
                                                       false))   // no timer colour in ready col
                                    .ToList();

            return new CompositeKitchenViewModel
            {
                NewOrders = newOrders,
                PendingOrders = pendingItems,
                CompletedOrders = completedItems
            };
        }
    }
}
