using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class KitchenOrderService
    {

        private readonly KitchenOrderRepository _kitchenOrderRepository;
        private readonly MenuItemRepository _menuItemRepository;
        public KitchenOrderService(KitchenOrderRepository kitchenOrderRepository)
        {
            _kitchenOrderRepository = kitchenOrderRepository;
        }

        public Order GetOrderById(int orderId)
        {
            Order order = _kitchenOrderRepository.GetOrderHeaderById(orderId); 
            if (order != null)
            {
                order.SetMenuItems(_kitchenOrderRepository.GetOrderItemsByOrderId(orderId));
            }
            return order!;
        }

        public List<Order> GetOrderByStatus(OrderStatus status) // For new order
        {
            List<Order> orders = _kitchenOrderRepository.GetOrderHeadersByStatus(status);
            foreach (Order order in orders)
            {
                order.SetMenuItems( _kitchenOrderRepository.GetOrderItemsByOrderId(order.Id));
            }
            return orders;
        }

        public List<OrderItem> GetMenuItemsOrder(OrderStatus status,int id)
        {

            return _kitchenOrderRepository.GetOrderItemsByOrderId(id);
        }

        public void UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            _kitchenOrderRepository.UpdateOrderStatus(orderId, orderStatus);
        }

        public void RemoveOrderFromDashboard(List<int> orderId)
        {
            _kitchenOrderRepository.RemoveOrder(orderId);
        }

        public List<OrderItem> GetMenuItemsByOrderId(int orderId)
        {
            return _kitchenOrderRepository.GetOrderItemsByOrderId(orderId);
        }



        //testing
        // ▼ ADD anywhere in the class body

        public List<OrderItem> GetMenuItemsByStatus(OrderStatus status)
            => _kitchenOrderRepository.GetOrderItemsByStatus(status);

        public void UpdateOrderItemStatus(int orderId, int menuItemId, OrderStatus newStatus)
            => _kitchenOrderRepository.UpdateOrderItemStatus(orderId, menuItemId, newStatus);


    }
}
