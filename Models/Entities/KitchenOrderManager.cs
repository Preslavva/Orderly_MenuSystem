namespace Models.Entities
{
    public class KitchenOrderManager
    {
        public List<Order> NewOrders { get; private set; }
        public List<Order> PendingOrders { get; private set; }
        public List<Order> CompletedOrders { get; private set; }

        public KitchenOrderManager()
        {
            NewOrders = new List<Order>();
            PendingOrders = new List<Order>();
            CompletedOrders = new List<Order>();
        }

        public void SetNewOrders(List<Order> newOrders)
        {
            NewOrders = newOrders;
        }

        public void SetPendingOrders(List<Order> pendingOrders)
        {
            PendingOrders = pendingOrders;
        }

        public void SetCompletedOrders(List<Order> completedOrders)
        {
            CompletedOrders = completedOrders;
        }
    }
}
