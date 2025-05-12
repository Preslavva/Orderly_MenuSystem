using Models.Entities;
using System.Globalization;

namespace MainOrderly.WebApp.ViewModels
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public string ItemPriceFormatted => ItemPrice.ToString("C", CultureInfo.CurrentCulture);




        public static CartViewModel ConvertToViewModel(MenuItem item, int quantity)
        {
            return new CartViewModel()
            {
                Id = item.Id,
                Picture = item.Picture,
                Name = item.Name,
                ItemPrice = item.Price,
                Quantity = quantity
            };
        }
    }
}
