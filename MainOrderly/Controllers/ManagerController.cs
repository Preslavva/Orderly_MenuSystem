using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    //https://localhost:7196/Manager/Create
    [RequireRole("Owner", "Manager" , "Chef" , "Waiter")]
    public class ManagerController : Controller
    {
        private readonly MenuService _menuService;
        private readonly IngredientService _ingredientService;
        private readonly NutritionService _nutritionService;
        private readonly TableService _tableService;
        private readonly QRCodeService _qrCodeService;
        private readonly int _restaurantId = 1;
        private readonly string _baseUrl;

        public ManagerController(MenuService menuService, IngredientService ingredientService, NutritionService nutritionService, TableService tableService, QRCodeService qrCodeService, IConfiguration configuration)
        {
            _menuService = menuService;
            _ingredientService = ingredientService;
            _nutritionService = nutritionService;
            _tableService = tableService;
            _qrCodeService = qrCodeService;
            _baseUrl = configuration["AppSettings:BaseUrl"]!;
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
            ModelState.Remove("Picture");
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 2 * 1024 * 1024) 
                {
                    ModelState.AddModelError("ImageFile", "Image must be less than 2MB.");
                    FillAvailableIngredients(model);
                    return View("Create", model);
                }

                var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                var extension = Path.GetExtension(model.ImageFile.FileName);
                var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine("wwwroot/images", uniqueName);
              
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                model.Picture = "/images/" + uniqueName; 
            }
            var nutritionDict = model.NutritionValues.ToDictionary(e => e.Name, e => e.Value);
            int menuId = _menuService.CreateMenuItem(
      model.Name,
      model.Description,
      model.Price,
      model.IsAvailable,
      model.Picture,
      model.Category,
      _restaurantId,
      nutritionDict, 
      model.SelectedAllergens
  );

            bool result = _menuService.AddIngridientsToMenuItem(
                menuId,
                model.SelectedIngredientIds.ToArray(),
                model.IngredientQuantities
                      .Where(e => model.SelectedIngredientIds.Contains(e.Key))
                      .Select(e => e.Value)
                      .ToArray()
            );

            return RedirectToAction("MenuItems");
        }

        [HttpGet]
        public IActionResult MenuItems(string? category)
        {
            List<MenuItem> menuItems;

            if (!string.IsNullOrEmpty(category) && category.ToLower() != "all"
     && Enum.TryParse<Category>(category, true, out var parsedCategory))
            {
                menuItems = _menuService.GetMenuItemsByCategory(parsedCategory);
            }
            else
            {
                menuItems = _menuService.LoadMenuItems(_restaurantId);
            }

            var viewModels = menuItems.Select(MenuItemViewModel.ConvertToViewModel).ToList();
            ViewBag.SelectedCategory = category;
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
            var nutritions = _nutritionService.GetNutritionForMenuItem(id);
            Console.WriteLine($"Found {nutritions.Count} nutrition values for item {id}");
            var nutritionList = Enum.GetValues(typeof(NutritionName))
     .Cast<NutritionName>()
     .Select(nutritionName =>
     {
         var found = nutritions.FirstOrDefault(n => n.Name == nutritionName);
         return new NutritionEntry
         {
             Name = nutritionName,
             Value = found?.Value ?? 0
         };
     }).ToList();


            var allergens = _menuService.GetAllergensForMenuItem(id);

          

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
                AvailableIngredients = availableIngredients
         .Select(e => new IngredientViewModel { Id = e.Id, Name = e.Name }).ToList(),
                IngredientQuantities = item.Ingredients.ToDictionary(i => i.IngredientId, i => i.Quantity),

                NutritionValues = nutritionList,
                SelectedAllergens = allergens
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

            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image must be less than 2MB.");
                    FillAvailableIngredients(model);
                    return View("EditMenuItem", model);
                }

                var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                var extension = Path.GetExtension(model.ImageFile.FileName);
                var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine("wwwroot/images", uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                model.Picture = "/images/" + uniqueName;
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
            var nutritionDict = model.NutritionValues
    .ToDictionary(e => e.Name, e => e.Value);

            _menuService.UpdateMenuItem(updated);

            _nutritionService.UpdateNutritions(model.Id, nutritionDict);
            _menuService.UpdateMenuItemAllergens(model.Id, model.SelectedAllergens);


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
        public IActionResult AddIngredient([FromBody] IngredientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(string.Join("; ", errors));
            }

            var ingredient = new Ingredient(
                id: 0,
                name: model.Name,
                unit: model.Unit,
                quantityInStock: model.QuantityInStock,
                minimumStockLevel: model.MinimumStockLevel,
                restaurantId: 1
            );

            _ingredientService.AddIngredient(ingredient);
            return Json(new { success = true });
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
        
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "BusinessAccount");
        }

        [HttpPut]
        public IActionResult UpdateIngredient([FromBody] IngredientViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            var updated = new Ingredient(
                id: model.Id,
                name: model.Name,
                unit: model.Unit,
                quantityInStock: model.QuantityInStock,
                minimumStockLevel: model.MinimumStockLevel,
                restaurantId: 1
            );

            _ingredientService.UpdateIngredient(updated);

            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientService.GetIngredientById(id);
            if (ingredient == null)
                return NotFound("Ingredient not found.");

            _ingredientService.DeleteIngredient(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            int restaurantId = 1; 
            var lowStock = _ingredientService.GetLowStockIngredients(restaurantId);

            var notifications = lowStock.Select(i => new
            {
                title = $"Low stock: {i.Name}",
                time = DateTime.Now.ToString("HH:mm, dd MMM"),
                link = "/Manager/IngredientList"
            }).ToList();

            return Json(notifications);
        }
        [HttpGet]
        public IActionResult TableList()
        {
            int restaurantId = 1;
            var tables = _tableService.GetTablesByRestaurantId(restaurantId);

            var viewModelList = tables.Select(t => new TableViewModel
            {
                Id = t.Id,
                TableNumber = t.Number
            }).ToList();

            return View(viewModelList);
        }

        [HttpGet]
        public IActionResult AddTable()
        {
            int restaurantId = 1;
            var tables = _tableService.GetTablesByRestaurantId(restaurantId);

            return View(tables.Count);
        }

        [HttpPost]
        public IActionResult AddNewTable(int tableNumber)
        {
            string guidToken = Guid.NewGuid().ToString();
            string qrUrl = $"{_baseUrl}/Home/LoadingPage?token={guidToken}";
            byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrUrl);

            var table = new Table(qrCodeImage, guidToken, tableNumber);
            _tableService.CreateTableWitNumber(table);

            return RedirectToAction("TableList");
        }

        [HttpGet]
        public IActionResult TableQR(int tableId)
        {
            if (tableId == 0)
            {
                return View("Error"); 
            }

            var qrCode = _tableService.GetTableQRById(tableId);
            var tableNum = _tableService.GetTableNumberById(tableId);

            if(qrCode == null || tableNum == null)
            {
                return View("Error");
            }
            var model = new TableQrViewModel
            {
                TableNumber = (int)tableNum,
                QrCode = qrCode
            };

            return PartialView("TableQR", model);

        }
    }
}
