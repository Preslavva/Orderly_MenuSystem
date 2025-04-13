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

        public int AddIngredient(Ingredient ingredient)
        {
            return _ingredientRepository.AddIngredient(ingredient);
        }

        public bool UpdateMenuItemIngredients(int menuItemId, Dictionary<int, decimal> ingredientQuantities)
        {
            return _ingredientRepository.UpdateMenuItemIngredients(menuItemId, ingredientQuantities);
        }


    }
}
