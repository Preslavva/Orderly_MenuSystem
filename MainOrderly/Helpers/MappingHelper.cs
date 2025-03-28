using MainOrderly.WebApp.ViewModels;
using Models.Entities;

namespace MainOrderly.WebApp.Helpers
{
    public class MappingHelper
    {
        public static List<MenuItemViewModel> ConvertToViewModels(List<MenuItem> menuItems)
        {
            return menuItems.Select(MenuItemViewModel.ConvertToViewModel).ToList();
        }
    }
}
