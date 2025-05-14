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
        public bool IsExceeded { get; set; }
        public string ElapsedTime { get; set; } = "00:00";
        public List<OrderLineItemViewModel> Items { get; set; }

        public OrderViewModel(List<OrderLineItemViewModel> items) => Items = items;
        public OrderViewModel() => Items = new();

   
        public static OrderViewModel ConvertToViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                Table = order.Table?.Id ?? 0, // adapt if you store table number differently
                OrderTimestamp = order.OrderTimestamp,
                Status = order.Status,
                Quantity = order.Items.Sum(i => i.Quantity),
                SubTotal = order.Items.Sum(i => i.MenuItem.Price * i.Quantity),
                Items = order.Items
                    .Select(oi => OrderLineItemViewModel.FromOrderItem(
                        oi,
                        order.Table?.Id ?? 0,
                        "00:00", // timer not running yet for NEW orders
                        false))
                    .ToList()
            };
        }
    }
}