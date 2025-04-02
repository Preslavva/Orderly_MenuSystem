using Microsoft.AspNetCore.Mvc.Formatters;
using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class MenuItemIngredientViewModel
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string IngredientUnit { get; set; }
        public int Quantity { get; set; } // required
        public int StockAvailable { get; set; }
        public string FormattedQuantity => $"{Quantity} {IngredientUnit}";

        public static MenuItemIngredientViewModel ConvertToViewModel(MenuItemIngredient menuItemIngredient)
        {
            return new MenuItemIngredientViewModel
            {
                IngredientId = menuItemIngredient.IngredientId,
                IngredientName = menuItemIngredient.Ingredient.Name,
                IngredientUnit = menuItemIngredient.Ingredient.Unit,
                Quantity = menuItemIngredient.Quantity,
                StockAvailable = menuItemIngredient.Ingredient.QuantityInStock,

            };          
        }
    }

    
}
