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

        public MenuService(MenuItemRepository menuItemRepository, IngredientRepository ingredientRepository,KitchenOrderService kitchenOrderService)
        {
            _menuItemRepository = menuItemRepository;
            _ingredientRepository = ingredientRepository;

            _kitchenOrderService = kitchenOrderService;

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

        public int CalculateAvgPrepTime(int menuItemId, int restaurantId) // this may not be needed at all
        {
            var menuItem = _menuItemRepository.GetMenuItemById(menuItemId, restaurantId);
            return  menuItem.PrepTime/* * quantity*/; // of the menuItem
        }

        public int CalculateOrderPrepTime(int orderId)
        {
            var order = _kitchenOrderService.GetOrderById(orderId);
            int maxPrepTime = 0;

            int totalPrepTime = 0;
            foreach (var item in order.Items)
            {
                totalPrepTime += item.MenuItem.PrepTime * item.Quantity;
            }

            return totalPrepTime;

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


    }
}
