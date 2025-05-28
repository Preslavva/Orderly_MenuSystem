using Models.Entities;
using MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class IngredientService
    {
        private readonly IngredientRepository _ingredientRepository;

        public IngredientService(IngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public Ingredient GetIngredientById(int id, int restaurantId)
        {
            return _ingredientRepository.GetIngredientById(id, restaurantId);
        }

        public List<Ingredient> GetIngredientsByRestaurantId(int restaurantId)
        {
            return _ingredientRepository.GetIngredientsByRestaurantId(restaurantId);
        }

        public List<Ingredient> GetIngredientsForItem(int menuItemId, int restaurantId)
        {
            return _ingredientRepository.GetIngredientsForItem(menuItemId, restaurantId);
        }

        public List<Ingredient> GetIngredientsForItemOnlyName(int menuItemId, int restaurantId) 
        {
            return _ingredientRepository.GetIngredientsForItemOnlyName(menuItemId, restaurantId);
        }

        public int AddIngredient(Ingredient ingredient)
        {
            return _ingredientRepository.AddIngredient(ingredient);
        }

        public bool UpdateMenuItemIngredients(int menuItemId, Dictionary<int, decimal> ingredientQuantities, int restaurantId)
        {
            return _ingredientRepository.UpdateMenuItemIngredients(menuItemId, ingredientQuantities, restaurantId);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            _ingredientRepository.UpdateIngredient(ingredient);
        }
        
        public void DeleteIngredient(int id, int restaurantId)
        {
            _ingredientRepository.Delete(id, restaurantId);
        }

        public List<Ingredient> GetLowStockIngredients(int restaurantId)
        {
            var ingredients = _ingredientRepository.GetIngredientsByRestaurantId(restaurantId);
            return ingredients.Where(i => i.QuantityInStock < i.MinimumStockLevel).ToList();
        }
        
        public void SubstractStock(MenuItem menuItem, int restaurantId)
        {
            menuItem.Ingredients.ForEach(ingredient =>
            {
                var ingredientToUpdate = _ingredientRepository.GetIngredientById(ingredient.IngredientId, restaurantId);
                if(ingredientToUpdate.QuantityInStock < ingredient.Quantity)
                {
                    menuItem.SetMenuItemAvailability(false);
                    throw new Exception($"Not enough stock for ingredient {ingredientToUpdate.Name}");
                }
                var quantityToSubstract = ingredient.Quantity;
                _ingredientRepository.SubstractIngredientStock(ingredient.IngredientId, quantityToSubstract, restaurantId);
            });
        }

        public List<MenuItemIngredient> GetIngredientForMenuItem_MenuItemIngredient(int id, int restaurantId)
        {
            return _ingredientRepository.GetIngredientsForMenuItem(id, restaurantId);
        }
    }
}
