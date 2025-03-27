using Models.Entities;
using MSSQL;

namespace Services
{
    public class MenuService
    {
        private readonly MenuItemRepository _menuItemRepository;

        public MenuService(MenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }


        public List<MenuItem> LoadMenuItems()
        {
           return _menuItemRepository.LoadMenuItems()!;   
        }

        public Models.Entities.MenuItem GetMenuItem(int id)
        {
            return _menuItemRepository.GetMenuItemById(id)!;
        }
    }
}
