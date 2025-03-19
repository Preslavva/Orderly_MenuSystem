namespace Models.Entities
{
    public class KitchenOrderManager
    {
        public List<Order> NewOrders { get; set; }
        public List<Order> PendingOrders { get; set; }
        public List<Order> CompletedOrders { get; set; }

        public KitchenOrderManager()
        {
            NewOrders = new List<Order>();
            PendingOrders = new List<Order>();
            CompletedOrders = new List<Order>();
        }
    }
}
