using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    //https://localhost:7196/Manager/Create
    public class ManagerController : Controller
    {
        private readonly MenuService _menuService;
        private readonly IngredientService _ingredientService;
        private readonly NutritionService _nutritionService;
        private readonly TableService _tableService;
        private readonly int _restaurantId = 1;

        public ManagerController(MenuService menuService, IngredientService ingredientService, NutritionService nutritionService, TableService tableService)
        {
            _menuService = menuService;
            _ingredientService = ingredientService;
            _nutritionService = nutritionService;
            _tableService = tableService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var ingridients = _ingredientService.GetIngredientsByRestaurantId(1).Select(e => new IngredientViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Unit = e.Unit,
                QuantityInStock = e.QuantityInStock,
                MinimumStockLevel = e.MinimumStockLevel
            }).ToList();

            return View(CreateMenuItemViewModel.Initialize(ingridients));
        }

        [HttpPost]
        public IActionResult Create(CreateMenuItemViewModel model)
        {
            ModelState.Remove("AvailableIngredients");
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            int menuId = _menuService.CreateMenuItem(
                model.Name,
                model.Description,
                model.Price,
                model.IsAvailable,
                model.Picture,
                model.Category,
                _restaurantId
            );

            bool result = _menuService.AddIngridientsToMenuItem(menuId, model.SelectedIngredientIds.ToArray(),
            model.IngredientQuantities.Where(e => model.SelectedIngredientIds.Contains(e.Key)).Select(e => e.Value).ToArray());


            return RedirectToAction("MenuItems");
        }

        [HttpGet]
        public IActionResult MenuItems()
        {
            var menuItems = _menuService.LoadMenuItems(_restaurantId);
            var viewModels = menuItems.Select(MenuItemViewModel.ConvertToViewModel).ToList();
            return View(viewModels);
        }

        [HttpGet]
        public IActionResult MenuItemDetails(int id)
        {
            var menuItem = _menuService.GetMenuItemWithIngredient(id, _restaurantId);
            var nutritions = _nutritionService.GetNutritionForMenuItem(id);
            var nutritionVMs = nutritions.Select(NutritionViewModel.ConvertToViewModel).ToList();

            var detailVM = MenuItemDetailViewModel.ConvertToViewModel(menuItem, nutritionVMs);
            return View(detailVM);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuService.GetMenuItemWithIngredient(id, _restaurantId);   
            var availableIngredients = _ingredientService.GetIngredientsByRestaurantId(_restaurantId);
            if (item == null)
            {
                return RedirectToAction("MenuItems");
            }
            var model = new CreateMenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Picture = item.Picture,
                IsAvailable = item.IsAvailable,
                Category = item.Category,
                AvailableIngredients = availableIngredients.Select(e => new IngredientViewModel { Id = e.Id, Name = e.Name }).ToList(),
                IngredientQuantities = item.Ingredients.ToDictionary(i => i.IngredientId, i => i.Quantity)

            };
            return View("EditMenuItem", model);

        }
        [HttpPost]
        public IActionResult Edit(CreateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FillAvailableIngredients(model);

                return View("EditMenuItem", model);
            }

            var updated = new MenuItem(
    model.Id,
    model.Name,
    model.Description,
    model.Price,
    model.IsAvailable,
    model.Picture,
    model.Category,
    _restaurantId,
    model.PrepTime
   
);

            _menuService.UpdateMenuItem(updated);

            var selectedIngredients = model.IngredientQuantities
                ?.Where(kv => kv.Value > 0)
                .ToDictionary(kv => kv.Key, kv => (decimal)kv.Value)
                ?? new Dictionary<int, decimal>();

            _ingredientService.UpdateMenuItemIngredients(model.Id, selectedIngredients);

            TempData["Success"] = "Menu item updated successfully!";
            return RedirectToAction("MenuItems");

        }


        private void FillAvailableIngredients(CreateMenuItemViewModel model)
        {
            var availableIngredients = _ingredientService.GetIngredientsByRestaurantId(_restaurantId);
            model.AvailableIngredients = availableIngredients
                .Select(e => new IngredientViewModel { Id = e.Id, Name = e.Name })
                .ToList();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool success = _menuService.DeleteMenuItem(id);
            if (!success)
            {
                TempData["Error"] = "Could not delete the menu item.";
            }
            else
            {
                TempData["Success"] = "Menu item deleted successfully.";
            }

            return RedirectToAction("MenuItems");
        }

        [HttpPost]
        public IActionResult AddIngredient(IngredientViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            var ingredient = new Ingredient(
                id: 0,
                name: model.Name,
                unit: model.Unit,
                quantityInStock: model.QuantityInStock,
                minimumStockLevel: model.MinimumStockLevel,
                restaurantId: 1 
            );

            _ingredientService.AddIngredient(ingredient);

            return Ok(); 
        }

        [HttpGet]
        public IActionResult IngredientList()
        {
            int restaurantId = 1;
            var ingredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            var viewModelList = ingredients.Select(i => new IngredientViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Unit = i.Unit,
                QuantityInStock = i.QuantityInStock,
                MinimumStockLevel = i.MinimumStockLevel
            }).ToList();

            return View(viewModelList);
        }

        [HttpGet]
        public IActionResult GetIngredientById(int id)
        {
            var ingredient = _ingredientService.GetIngredientById(id);
            if (ingredient == null) return NotFound();

            return Json(new
            {
                id = ingredient.Id,
                name = ingredient.Name,
                quantityInStock = ingredient.QuantityInStock,
                minimumStockLevel = ingredient.MinimumStockLevel,
                unit = ingredient.Unit
            });
        }

        [HttpGet]
        public IActionResult TableList()
        {
            int restaurantId = 1;
            var tables = _tableService.GetTablesByRestaurantId(restaurantId);

            var viewModelList = tables.Select(t => new TableViewModel
            {
                Id = t.Id,
                QrCode = t.QrCode
            }).ToList();

            return View(viewModelList);
        }

    }
}
