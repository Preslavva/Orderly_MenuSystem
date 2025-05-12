using Models.Entities;
using MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class IngredientService
    {
        private readonly IngredientRepository _ingredientRepository; // INTERFACES   

        public IngredientService(IngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public Ingredient GetIngredientById(int Id)
        {
            return _ingredientRepository.GetIngredientById(Id);
        }

        public List<Ingredient> GetIngredientsByRestaurantId(int restaurantId)
        {
            return _ingredientRepository.GetIngredientsByRestaurantId(restaurantId);
        }

        public List<Ingredient> GetIngredientsForItem(int menuItemId)
        {
            return _ingredientRepository.GetIngredientsForItem(menuItemId);
        }

        public int AddIngredient(Ingredient ingredient)
        {
            return _ingredientRepository.AddIngredient(ingredient);
        }

        public bool UpdateMenuItemIngredients(int menuItemId, Dictionary<int, decimal> ingredientQuantities)
        {
            return _ingredientRepository.UpdateMenuItemIngredients(menuItemId, ingredientQuantities);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            _ingredientRepository.UpdateIngredient(ingredient);
        }
        public void DeleteIngredient(int id)
        {
            _ingredientRepository.Delete(id);
        }
       
        public List<Ingredient> GetLowStockIngredients(int restaurantId)
        {
            var ingredients = _ingredientRepository.GetIngredientsByRestaurantId(restaurantId);
            return ingredients.Where(i => i.QuantityInStock < i.MinimumStockLevel).ToList();
        }
        public void SubstractStock(MenuItem menuItem)
        {
            
            menuItem.Ingredients.ForEach(ingredient =>
            {
                var ingredientToUpdate= _ingredientRepository.GetIngredientById(ingredient.IngredientId);
                var quantityToSubstract = menuItem.Ingredients.Sum(ingredient => ingredient.Quantity);
                _ingredientRepository.SubstractIngredientStock(ingredient.IngredientId, quantityToSubstract);

            });
        }

    }
}
