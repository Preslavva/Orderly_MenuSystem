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

        public List<MenuItem> LoadMenuItems(int restaurantId)
        {
           return _menuItemRepository.LoadMenuItems(restaurantId)!;   
        }

        public MenuItem GetMenuItem(int id)
        {
            return _menuItemRepository.GetMenuItemById(id)!;
        }

        public MenuItem GetMenuItemWithIngredient(int id)
        {
            MenuItem menuItem = _menuItemRepository.GetMenuItemById(id);
            List<MenuItemIngredient> ingredient = _ingredientRepository.GetIngredientsForMenuItem(id);
            
            menuItem.SetIngredient(ingredient);
           
            return menuItem;
        }
    }
}
