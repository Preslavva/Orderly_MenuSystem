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

        public int CreateMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category, int restaurantId,
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
                _menuItemRepository.AddAllergenToMenuItem(menuItemId, allergen, restaurantId);
            }

            return menuItemId;
        }

        public bool AddIngridientsToMenuItem(int menuId, int[] ingredientIds, int[] quantities, int restaurantId)
        {
            return _menuItemRepository.AddMenuIngredients(menuId, ingredientIds, quantities, restaurantId);
        }

        public List<MenuItem> LoadMenuItems(int restaurantId)// for the user side
        {

            var menuItems = _menuItemRepository.LoadMenuItems(restaurantId);
            foreach (var item in menuItems)
                item.SetIngredient(_ingredientService.GetIngredientForMenuItem_MenuItemIngredient(item.Id, restaurantId));


            var available = new List<MenuItem>();

            foreach (var item in menuItems)
            {
                bool inStock = item.Ingredients.All(ing =>
                {
                    var inv = _ingredientService.GetIngredientById(ing.IngredientId, restaurantId);
                    return inv.QuantityInStock >= ing.Quantity;
                });

                bool isMarkedAvailable = item.IsAvailable;

                item.SetMenuItemAvailability(inStock && isMarkedAvailable);

                if (inStock && isMarkedAvailable)
                {
                    available.Add(item);
                }
            }

            return available;
        }


        public List<MenuItem> LoadMenuItemsForUser(int restaurantId)// for the user side
        {

            var menuItems = _menuItemRepository.LoadMenuItems(restaurantId);
            foreach (var item in menuItems)
                item.SetIngredient(_ingredientService.GetIngredientForMenuItem_MenuItemIngredient(item.Id, restaurantId));


           ;

            foreach (var item in menuItems)
            {
                bool inStock = item.Ingredients.All(ing =>
                {
                    var inv = _ingredientService.GetIngredientById(ing.IngredientId, restaurantId);
                    return inv.QuantityInStock >= ing.Quantity;
                });

                bool isMarkedAvailable = item.IsAvailable;

                item.SetMenuItemAvailability(inStock && isMarkedAvailable);
            }

            return menuItems;
        }


        public List<MenuItem> LoadMenuItemsForManager(int restaurantId)
        {
            var menuItems = _menuItemRepository.LoadMenuItems(restaurantId);
            foreach (var item in menuItems)
                item.SetIngredient(_ingredientService.GetIngredientForMenuItem_MenuItemIngredient(item.Id, restaurantId));

            foreach (var item in menuItems)
            {

                bool inStock = item.Ingredients.All(ing =>
                {
                    var inv = _ingredientService.GetIngredientById(ing.IngredientId, restaurantId);
                    return inv != null && inv.QuantityInStock >= ing.Quantity;
                });

                //item. item.HasLowStockIngredients = !hasAllIngredients;

                if (!item.IsAvailable || !inStock)
                {
                    item.SetMenuItemAvailability(false); // ✅ override to false if any understocked
                }
            
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
            Console.WriteLine("IsAvailable: " + item?.IsAvailable);
            if (item == null)
            {
                return null; 
            }

            var ingredients = _ingredientRepository.GetIngredientsForMenuItem(id, restaurantId) ?? new List<MenuItemIngredient>();
            item.SetIngredient(ingredients);

            return item;
        }

        public bool DeleteMenuItem(int menuItemId, int restaurantId)
        {
            return _menuItemRepository.DeleteMenuItem(menuItemId, restaurantId);
        }

        public MenuItem UpdateMenuItem(MenuItem updatedItem, int restaurantId)
        {
            _menuItemRepository.UpdateMenuItem(updatedItem, restaurantId);

            int id = updatedItem.Id;
            MenuItem menuItem = _menuItemRepository.GetMenuItemById(id, restaurantId);
            List<MenuItemIngredient> ingredient = _ingredientRepository.GetIngredientsForMenuItem(id, restaurantId);
            menuItem.SetIngredient(ingredient);

            return menuItem;
        }

        public void UpdateMenuItemAllergens(int menuItemId, List<AllergenName> allergens, int restaurantId)
        {
            _menuItemRepository.DeleteAllergensForMenuItem(menuItemId, restaurantId);

            foreach (var allergen in allergens)
            {
                _menuItemRepository.AddAllergenToMenuItem(menuItemId, allergen, restaurantId);
            }
        }
        
        public List<AllergenName> GetAllergensForMenuItem(int menuItemId, int restaurantId)
        {
            return _menuItemRepository.GetAllergensForMenuItem(menuItemId, restaurantId);
        }
        
        public List<MenuItem> GetMenuItemsByCategory(Category category, int restaurantId)
        {
            return _menuItemRepository.LoadMenuItemsByCategory(category, restaurantId);
        }
    }
}