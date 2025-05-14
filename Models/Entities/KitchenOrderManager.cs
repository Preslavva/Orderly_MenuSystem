namespace Models.Entities
{
    public class KitchenOrderManager
    {
        public List<Order> NewOrders { get; private set; }
        public List<OrderItem> PendingOrders { get; private set; }
        public List<OrderItem> CompletedOrders { get; private set; }

        public KitchenOrderManager()
        {
            NewOrders = new List<Order>();
            PendingOrders = new List<OrderItem>();
            CompletedOrders = new List<OrderItem>();
        }

        public void SetNewOrders(List<Order> newOrders)
        {
            NewOrders = newOrders;
        }

        public void SetPendingOrders(List<OrderItem> pendingOrders)
        {
            PendingOrders = pendingOrders;
        }

        public void SetCompletedOrders(List<OrderItem> completedOrders)
        {
            CompletedOrders = completedOrders;
        }
    }
}
