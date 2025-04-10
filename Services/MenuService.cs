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

        public List<MenuItem> LoadMenuItems(int restaurantId)
        {
           return _menuItemRepository.LoadMenuItems(restaurantId)!;   
        }

        public MenuItem GetMenuItem(int id)
        {
            return _menuItemRepository.GetMenuItemById(id)!;
        }

        public int CalculateAvgPrepTime(int menuItemId, int quantity) // this may not be needed at all
        {
            var menuItem = _menuItemRepository.GetMenuItemById(menuItemId);
            return  menuItem.PrepTime * quantity; // of the menuItem
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

        public MenuItem GetMenuItemWithIngredient(int id)
        {
            MenuItem menuItem = _menuItemRepository.GetMenuItemById(id);
            List<MenuItemIngredient> ingredient = _ingredientRepository.GetIngredientsForMenuItem(id);
            
            menuItem.SetIngredient(ingredient);
           
            return menuItem;
        }

    }
}
