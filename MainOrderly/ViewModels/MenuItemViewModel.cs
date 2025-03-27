using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public string Picture { get; set; }

        public int Quantity { get; set; }

        public Continent Continent { get; set; }
        public List<int> Nutritions { get; set; }

        public MenuItemViewModel()
        {
            this.Nutritions = new List<int>();         
        }

        public static MenuItemViewModel ConvertToViewModel(MenuItem menuItem)
        {
            MenuItemViewModel viewModel = new MenuItemViewModel()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                Picture = menuItem.Picture,
                Quantity = menuItem.Quantity,
                Continent = menuItem.Continent,

            };

            foreach(var item in menuItem.Nutritions)
            {
                viewModel.Nutritions.Add(item.Id);
            }

            return viewModel;
        }






    }
}
