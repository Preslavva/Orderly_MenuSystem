using Models.Entities;
using Models.Enums;
using MSSQL;

namespace Services
{
    public class MenuService
    {
        private readonly MenuItemRepository _menuItemRepository;
        private readonly IngredientRepository _ingredientRepository;

        public MenuService(MenuItemRepository menuItemRepository, IngredientRepository ingredientRepository)
        {
            _menuItemRepository = menuItemRepository;
            _ingredientRepository = ingredientRepository;

        }

        public int CreateMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category, int restaurantId)
        {
            return _menuItemRepository.AddMenuItem(name, description, price, isAvailable, picture, category, restaurantId);
        }

        public bool AddIngridientsToMenuItem(int menuId, int[] ingredientIds, int[] quantities)
        {
            return _menuItemRepository.AddMenuIngredients(menuId, ingredientIds, quantities);
        }

        public List<MenuItem> LoadMenuItems(int restaurantId)
        {
            return _menuItemRepository.LoadMenuItems(restaurantId)!;
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

        public void UpdateMenuItem(MenuItem updatedItem)
        {
            _menuItemRepository.UpdateMenuItem(updatedItem);
        }

    }
}
