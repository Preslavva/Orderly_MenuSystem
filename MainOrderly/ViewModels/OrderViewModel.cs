using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int Table { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public OrderStatus Status { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }

        public List<MenuItemViewModel> Items { get; set; }

        public OrderViewModel(List<MenuItemViewModel> menuItemViewModels)
        {
            Items = menuItemViewModels;
        }

        public OrderViewModel()
        {
            
        }

        public static OrderViewModel ConvertToViewModel(Order order)
        {
            OrderViewModel viewModel = new OrderViewModel()
            {
                Id = order.Id,
                Table = order.Table.Id,
                OrderTimestamp = order.OrderTimestamp,
                Status = order.Status,
                Quantity = order.Quantity,
                SubTotal = order.SubTotal,
                Items = order.Items.Select(o=> MenuItemViewModel.ConvertToViewModel(o)).ToList()


            };


            return viewModel;
        }
    }
}
