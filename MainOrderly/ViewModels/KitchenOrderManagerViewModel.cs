using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class KitchenOrderManagerViewModel
    {
        public List<Order> NewOrders { get; set; }
        public List<Order> PendingOrders { get; set; }
        public List<Order> CompletedOrders { get; set; }

        public KitchenOrderManagerViewModel()
        {
            this.NewOrders = new List<Order>();
            this.PendingOrders = new List<Order>();
            this.CompletedOrders = new List<Order>();
        }


        public static KitchenOrderManagerViewModel FromEntity(KitchenOrderManager kitchenOrderManager)
        {
            return new KitchenOrderManagerViewModel
            {
                NewOrders = kitchenOrderManager.NewOrders,
                PendingOrders = kitchenOrderManager.PendingOrders,
                CompletedOrders = kitchenOrderManager.CompletedOrders
            };
        }



    }
}
