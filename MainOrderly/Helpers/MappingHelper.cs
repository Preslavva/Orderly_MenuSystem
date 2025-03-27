using MainOrderly.WebApp.ViewModels;

namespace MainOrderly.WebApp.Helpers
{
    public class MappingHelper
    {
        public static List<MenuItemViewModel> ConvertToViewModels(List<MenuItemDTO> menuItems)
        {
            return menuItems.Select(MenuItemViewModel.ConvertToViewModel).ToList();
        }
    }
}
