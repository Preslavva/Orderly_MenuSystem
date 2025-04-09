
using Models;
using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class KitchenOrderService
    {

        private readonly KitchenOrderRepository _kitchenOrderRepository;
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

        public List<Order> GetOrderByStatus(OrderStatus status)
        {
            List<Order> orders = _kitchenOrderRepository.GetOrderHeadersByStatus(status);
            foreach (Order order in orders)
            {
                order.SetMenuItems( _kitchenOrderRepository.GetOrderItemsByOrderId(order.Id));
            }
            return orders;
        }

        public void UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            _kitchenOrderRepository.UpdateOrderStatus(orderId, orderStatus);
        }

        public void RemoveOrderFromDashboard(int orderId)
        {
            _kitchenOrderRepository.RemoveOrder(orderId);
        }

        public CountdownTimer UpdatePrepTime(int orderId, int menuItemId, int prepTime, DateTime startTime)
        {
           
            var actualStartTime = DateTime.Now;

            double elapsedMinutes = (DateTime.Now - actualStartTime).TotalMinutes;

            int remainingTime = Math.Max(0, prepTime - (int)elapsedMinutes);
           
             CountdownTimer countdownTimer = new CountdownTimer
            {
                OrderId = orderId,
                MenuItemId = menuItemId,
                RemainingTimeInMinutes = remainingTime,
            };
            
            return countdownTimer;


        }
    
    }
}
