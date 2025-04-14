using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class CreateMenuItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Picture { get; set; }
        public int PrepTime { get; set; }
        public Category Category { get; set; }

        [BindNever, ValidateNever]
        public List<IngredientViewModel> AvailableIngredients { get; set; }
        public List<int> SelectedIngredientIds { get; set; }
        public Dictionary<int, int> IngredientQuantities { get; set; }

        public static CreateMenuItemViewModel Initialize(List<IngredientViewModel> availableIngredients)
        {
            return new CreateMenuItemViewModel
            {
                IsAvailable = true, 
                AvailableIngredients = availableIngredients,
                SelectedIngredientIds = new List<int>(),
                IngredientQuantities = new Dictionary<int, int>()
            };
        }


    }
}
