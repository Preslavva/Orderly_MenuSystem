using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class MenuService
    {
        private readonly MenuItemRepository _menuItemRepository;
        private readonly IngredientRepository _ingredientRepository;
        private readonly KitchenOrderService _kitchenOrderService;
        private readonly NutritionRepository _nutritionRepository;
        private readonly IngredientService _ingredientService;

        public MenuService(MenuItemRepository menuItemRepository, IngredientRepository ingredientRepository, KitchenOrderService kitchenOrderService, NutritionRepository nutritionRepository, IngredientService ingredientService)
        {
            _menuItemRepository = menuItemRepository;
            _ingredientRepository = ingredientRepository;
            _kitchenOrderService = kitchenOrderService;
            _nutritionRepository = nutritionRepository;
            _ingredientService = ingredientService;
        }

        public int CreateMenuItem(string name,string description, decimal price,bool isAvailable, string picture,Category category, int restaurantId,
     Dictionary<NutritionName, double> nutritionValues,
     List<AllergenName> allergens, int prepTime)
        {
            int menuItemId = _menuItemRepository.AddMenuItem(name, description, price, isAvailable, picture, category, restaurantId, prepTime);

            foreach (var nutrition in nutritionValues)
            {
                int nutritionId = _nutritionRepository.AddNutrition(nutrition.Key.ToString(), (int)nutrition.Value);
                _nutritionRepository.AssignNutritionToMenuItem(menuItemId, nutritionId);
            }

            foreach (var allergen in allergens)
            {
                _menuItemRepository.AddAllergenToMenuItem(menuItemId, allergen);
            }

            return menuItemId;
        }



        public bool AddIngridientsToMenuItem(int menuId, int[] ingredientIds, int[] quantities)
        {
            return _menuItemRepository.AddMenuIngredients(menuId, ingredientIds, quantities);
        }

        public List<MenuItem> LoadMenuItems(int restaurantId)// for the user side
        {

            var menuItems = _menuItemRepository.LoadMenuItems(restaurantId);
            foreach (var item in menuItems)
                item.SetIngredient(_ingredientService.GetIngredientForMenuItem_MenuItemIngredient(item.Id));


            var available = new List<MenuItem>();

            foreach (var item in menuItems)
            {
                bool inStock = item.Ingredients.All(ing =>
                {
                    var inv = _ingredientService.GetIngredientById(ing.IngredientId);
                    return inv.QuantityInStock >= ing.Quantity;
                });

                item.SetMenuItemAvailability(inStock);

                if (inStock)
                    available.Add(item);           
            }

            return available;
        }

        public List<MenuItem> LoadMenuItemsForManager(int restaurantId)// for the manager side
        {

            var menuItems = _menuItemRepository.LoadMenuItems(restaurantId);
            foreach (var item in menuItems)
                item.SetIngredient(_ingredientService.GetIngredientForMenuItem_MenuItemIngredient(item.Id));



            foreach (var item in menuItems)
            {
                bool inStock = item.Ingredients.All(ing =>
                {
                    var inv = _ingredientService.GetIngredientById(ing.IngredientId);
                    return inv.QuantityInStock >= ing.Quantity;
                });

                item.SetMenuItemAvailability(inStock);

            }

            return menuItems;
        }

        public MenuItem GetMenuItem(int id, int restaurantId)
        {
            return _menuItemRepository.GetMenuItemById(id, restaurantId)!;
        }


        public MenuItem GetMenuItemWithIngredient(int id, int restaurantId)
        {
            var item = _menuItemRepository.GetMenuItemById(id, restaurantId);

            if (item == null)
            {
                return null; 
            }

            var ingredients = _ingredientRepository.GetIngredientsForMenuItem(id) ?? new List<MenuItemIngredient>();
            item.SetIngredient(ingredients);

            return item;
        }

        public bool DeleteMenuItem(int menuItemId)
        {
            return _menuItemRepository.DeleteMenuItem(menuItemId);
        }

        public MenuItem UpdateMenuItem(MenuItem updatedItem)
        {
            _menuItemRepository.UpdateMenuItem(updatedItem);

            int id = updatedItem.Id;
            int restaurantId = 1;
            MenuItem menuItem = _menuItemRepository.GetMenuItemById(id, restaurantId);
            List<MenuItemIngredient> ingredient = _ingredientRepository.GetIngredientsForMenuItem(id);
            menuItem.SetIngredient(ingredient);

            return menuItem;
        }

        public void UpdateMenuItemAllergens(int menuItemId, List<AllergenName> allergens)
        {
            _menuItemRepository.DeleteAllergensForMenuItem(menuItemId);

            foreach (var allergen in allergens)
            {
                _menuItemRepository.AddAllergenToMenuItem(menuItemId, allergen);
            }
        }
        public List<AllergenName> GetAllergensForMenuItem(int menuItemId)
        {
            return _menuItemRepository.GetAllergensForMenuItem(menuItemId);
        }
        public List<MenuItem> GetMenuItemsByCategory(Category category)
        {
            return _menuItemRepository.LoadMenuItemsByCategory(category);
        }

     



    }
}
