using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class CompositeKitchenViewModel
    {
        public List<OrderViewModel> NewOrders { get; set; }
        public List<OrderViewModel> PendingOrders { get; set; }
        public List<OrderViewModel> CompletedOrders { get; set; }

        public CompositeKitchenViewModel()
        {
            this.NewOrders = new List<OrderViewModel>();
            this.PendingOrders = new List<OrderViewModel>();
            this.CompletedOrders = new List<OrderViewModel>();
        }

        public static CompositeKitchenViewModel ConvertToViewModel(KitchenOrderManager kitchenOrderManager)
        {
            return new CompositeKitchenViewModel
            {
                NewOrders = kitchenOrderManager.NewOrders.Select(o => OrderViewModel.ConvertToViewModel(o)).ToList(),
                PendingOrders = kitchenOrderManager.PendingOrders.Select(o => OrderViewModel.ConvertToViewModel(o)).ToList(),
                CompletedOrders = kitchenOrderManager.CompletedOrders.Select(o => OrderViewModel.ConvertToViewModel(o)).ToList()

            };

        }



    }
}
