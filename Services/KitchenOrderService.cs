
using Models;
using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class KitchenOrderService
    {

        private readonly KitchenOrderRepository _kitchenOrderRepositories;
        public KitchenOrderService(KitchenOrderRepository orderRepository)
        {
            _kitchenOrderRepositories = orderRepository;
        }

        public Order GetOrderById(int orderId)
        {
            Order order = _kitchenOrderRepositories.GetOrderHeaderById(orderId);
            if (order != null)
            {
                order.Items = _kitchenOrderRepositories.GetOrderItemsByOrderId(orderId);
            }
            return order!;
        }

        public List<Order> GetOrderByStatus(OrderStatus status)
        {
            List<Order> orders = _kitchenOrderRepositories.GetOrderHeadersByStatus(status);
            foreach (Order order in orders)
            {
                order.Items = _kitchenOrderRepositories.GetOrderItemsByOrderId(order.Id);
            }
            return orders;
        }


        public void UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            _kitchenOrderRepositories.UpdateOrderStatus(orderId, orderStatus);
        }

        public void RemoveOrderFromDashboard(int orderId)
        {
            _kitchenOrderRepositories.RemoveOrder(orderId);
        }
    }
}
