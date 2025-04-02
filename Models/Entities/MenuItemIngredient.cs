using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class MenuItemIngredient
    {
        public int MenuItemId { get; set; }
        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; }

        public int Quantity { get; set; } // quantity required

        public MenuItemIngredient(int menuItemId, int ingredientId, int quantity)
        {
            MenuItemId = menuItemId;
            IngredientId = ingredientId;
            Quantity = quantity;
        }

        public MenuItemIngredient(int menuItemId, int ingredientId, Ingredient ingredient, int quantity)
        {
            MenuItemId = menuItemId;
            IngredientId = ingredientId;
            Ingredient = ingredient;
            Quantity = quantity;
        }
    }
}
