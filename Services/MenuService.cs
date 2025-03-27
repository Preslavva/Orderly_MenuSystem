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


        public List<MenuItemDTO> LoadMenuItems()
        {
            List<MenuItem> menuItems = _menuItemRepository.LoadMenuItems()!; // should return dto

            return menuItems.Select(MenuItemDTO.ConvertToDTO).ToList();
        }

        public Models.Entities.MenuItem GetMenuItem(int id)
        {
            return _menuItemRepository.GetMenuItemById(id)!;
        }
    }
}
