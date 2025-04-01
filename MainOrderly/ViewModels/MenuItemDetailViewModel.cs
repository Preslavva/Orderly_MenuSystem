using Models.Entities;
using System.Globalization;

namespace MainOrderly.WebApp.ViewModels
{
    public class MenuItemDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice => Price.ToString("C", CultureInfo.CurrentCulture);
        public bool IsAvailable { get; set; }
        public string Picture { get; set; }
        public string Category { get; set; }
        public List<MenuItemIngredientViewModel> Ingredients { get; set; }
        public List<NutritionViewModel> Nutritions { get; set; }

        public MenuItemDetailViewModel()
        {
            this.Ingredients = new List<MenuItemIngredientViewModel>();
            this.Nutritions = new List<NutritionViewModel>();
        }
        public static MenuItemDetailViewModel ConvertToViewModel(MenuItem menuItem, List<NutritionViewModel> nutritionViewModels)
        {
            return new MenuItemDetailViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                Picture = menuItem.Picture,
                Category = menuItem.Category.ToString(),
                Nutritions = nutritionViewModels,
                Ingredients = menuItem.Ingredients.Select(i => MenuItemIngredientViewModel.ConvertToViewModel(i)).ToList()

            };
        }

    }
}
