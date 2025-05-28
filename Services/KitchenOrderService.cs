using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class KitchenOrderService
    {
        private readonly KitchenOrderRepository _kitchenOrderRepository;
        private readonly MenuItemRepository _menuItemRepository;
        
        public KitchenOrderService(KitchenOrderRepository kitchenOrderRepository, MenuItemRepository menuItemRepository)
        {
            _kitchenOrderRepository = kitchenOrderRepository;
            _menuItemRepository = menuItemRepository;
        }

        public Order GetOrderById(int orderId, int restaurantId)
        {
            Order order = _kitchenOrderRepository.GetOrderHeaderById(orderId, restaurantId); 
            if (order != null)
            {
                order.SetMenuItems(_kitchenOrderRepository.GetOrderItemsByOrderId(orderId, restaurantId));
            }
            return order!;
        }

        public List<Order> GetOrderByStatus(OrderStatus status, int restaurantId) // For new order
        {
            List<Order> orders = _kitchenOrderRepository.GetOrderHeadersByStatus(status, restaurantId);
            foreach (Order order in orders)
            {
                order.SetMenuItems(_kitchenOrderRepository.GetOrderItemsByOrderId(order.Id, restaurantId));
            }
            return orders;
        }

        public List<OrderItem> GetMenuItemsOrder(OrderStatus status, int id, int restaurantId)
        {
            return _kitchenOrderRepository.GetOrderItemsByOrderId(id, restaurantId);
        }

        public void UpdateOrderStatus(int orderId, OrderStatus orderStatus, int restaurantId)
        {
            _kitchenOrderRepository.UpdateOrderStatus(orderId, orderStatus, restaurantId);
        }

        public void RemoveOrderFromDashboard(List<int> orderId, int restaurantId)
        {
            _kitchenOrderRepository.RemoveOrder(orderId, restaurantId);
        }

        public List<OrderItem> GetMenuItemsByOrderId(int orderId, int restaurantId)
        {
            return _kitchenOrderRepository.GetOrderItemsByOrderId(orderId, restaurantId);
        }
        
        public List<OrderItem> GetMenuItemsByStatus(OrderStatus status, int restaurantId)
            => _kitchenOrderRepository.GetOrderItemsByStatus(status, restaurantId);

        public void UpdateOrderItemStatus(int orderId, int menuItemId, OrderStatus newStatus, int restaurantId)
            => _kitchenOrderRepository.UpdateOrderItemStatus(orderId, menuItemId, newStatus, restaurantId);
    }
}
