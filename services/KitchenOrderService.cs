using OrderlyTest.Models;
using OrderlyTest.repos;

namespace OrderlyTest.services
{
    public class KitchenOrderService
    {

        private readonly OrderRepositories _orderRepositories;
        public KitchenOrderService(OrderRepositories orderRepository)
        {
            _orderRepositories = orderRepository;   
        }

        public List<Order> GetOrderByStatus(OrderStatus status)
        {
           return _orderRepositories.GetOrdersByStatus(status);
        }

        public Order GetOrderById(int orderId)
        {
            return _orderRepositories.GetOrderById(orderId);
        }

        public void  UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            _orderRepositories.UpdateOrderStatus(orderId, orderStatus);
        }
    }
}
