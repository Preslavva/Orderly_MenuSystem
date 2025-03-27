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

        public static MenuItemViewModel ConvertToViewModel(MenuItemDTO menuItemDTO)
        {
            MenuItemViewModel viewModel = new MenuItemViewModel()
            {
                Id = menuItemDTO.Id,
                Name = menuItemDTO.Name,
                Description = menuItemDTO.Description,
                Price = menuItemDTO.Price,
                IsAvailable = menuItemDTO.IsAvailable,
                Picture = menuItemDTO.Picture,
                Quantity = menuItemDTO.Quantity,
                Continent = menuItemDTO.Continent,

            };

            foreach(var item in menuItemDTO.Nutritions)
            {
                viewModel.Nutritions.Add(item.Id);
            }

            return viewModel;
        }






    }
}
