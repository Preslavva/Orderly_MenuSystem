using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly MenuService _menuService;
        private readonly IngredientService _ingredientService;
        private readonly NutritionService _nutritionService;
        private readonly int _restaurantId = 1;

        public ManagerController(MenuService menuService, IngredientService ingredientService, NutritionService nutritionService)
        {
            _menuService = menuService;
            _ingredientService = ingredientService;
            _nutritionService = nutritionService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MenuItems()
        {
            List<MenuItem> menuItems = _menuService.LoadMenuItems(_restaurantId);
            List<MenuItemViewModel> menuItemViewModels = menuItems.Select(m=> MenuItemViewModel.ConvertToViewModel(m)).ToList();    
            return View(menuItemViewModels);
        }

        public IActionResult MenuItemDetails(int id, int restaurantId)
        {
            MenuItem menuItem = _menuService.GetMenuItemWithIngredient(id, restaurantId);
            List<Nutrition> nutritions = _nutritionService.GetNutritionForMenuItem(id);
            List<NutritionViewModel> nutritionViewModels = nutritions.Select(n => NutritionViewModel.ConvertToViewModel(n)).ToList();   
            
            MenuItemDetailViewModel menuItemDetailViewModel = MenuItemDetailViewModel.ConvertToViewModel(menuItem,nutritionViewModels);
            return View(menuItemDetailViewModel);
            
        }

        //public IActionResult CreateMenuItems(istring name,int restaurantId)
        //{
        //    MenuItem menuItem = _menuService.CreateMenuItem(restaurantId, id);
        //}
    }
}
