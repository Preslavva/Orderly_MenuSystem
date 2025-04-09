using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class CreateMenuItemViewModel
    {     
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Picture { get; set; }
        public Category Category { get; set; }
        public List<IngredientViewModel> AvailableIngredients { get; set; }
        public List<int> SelectedIngredientIds { get; set; }
        public Dictionary<int, decimal> IngredientQuantities { get; set; }

        public static CreateMenuItemViewModel Initialize(List<IngredientViewModel> availableIngredients)
        {
            return new CreateMenuItemViewModel
            {
                IsAvailable = true, 
                AvailableIngredients = availableIngredients,
                SelectedIngredientIds = new List<int>(),
                IngredientQuantities = new Dictionary<int, decimal>()
            };
        }


    }
}
