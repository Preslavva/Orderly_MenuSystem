namespace OrderlyTest.Models
{
    public class KitchenOrderManager
    {
        public List<Order> pendingOrders { get; set; }
        public List<Order> completedOrders { get; set; }

        public KitchenOrderManager()
        {
            pendingOrders = new List<Order>();
            completedOrders = new List<Order>();
        }
    }
}
