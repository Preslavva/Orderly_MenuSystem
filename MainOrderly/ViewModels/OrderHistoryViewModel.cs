using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public string Status { get; set; }
        public DateTime OrderTimeStamp { get; set; }



        public static OrderHistoryViewModel ConvertToViewModel(OrderHistory history)
        {
            OrderHistoryViewModel viewModel = new OrderHistoryViewModel()
            {
                Id = history.Id,
                Name = history.Name,
                Quantity = history.Quantity,
                Price = history.Price,
                SubTotal = history.SubTotal,
                Status = history.Status.ToString(),
                OrderTimeStamp = history.OrderTimeStamp
            };


            return viewModel;
        }
    }
}
