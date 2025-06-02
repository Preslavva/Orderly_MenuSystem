using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner", "Manager", "Chef", "Waiter")]
    public class ManagerController : Controller
    {
        private readonly MenuService _menuService;
        private readonly IngredientService _ingredientService;
        private readonly NutritionService _nutritionService;
        private readonly TableService _tableService;
        private readonly QRCodeService _qrCodeService;
        private readonly string _baseUrl;

        public ManagerController(MenuService menuService, IngredientService ingredientService, 
            NutritionService nutritionService, TableService tableService, 
            QRCodeService qrCodeService, IConfiguration configuration)
        {
            _menuService = menuService;
            _ingredientService = ingredientService;
            _nutritionService = nutritionService;
            _tableService = tableService;
            _qrCodeService = qrCodeService;
            _baseUrl = configuration["AppSettings:BaseUrl"]!;
        }

        private int GetCurrentRestaurantId()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            return user?.RestaurantId ?? 0;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int pageNumber = 1)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            const int pageSize = 5;
            var ingredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId)
                .Select(e => new IngredientViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Unit = e.Unit,
                    QuantityInStock = e.QuantityInStock,
                    MinimumStockLevel = e.MinimumStockLevel
                }).ToList();

            var pagedIngredients = ingredients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new CreateMenuItemViewModel
            {
                IsAvailable = true,
                AvailableIngredients = pagedIngredients,
                SelectedIngredientIds = new List<int>(),
                IngredientQuantities = new Dictionary<int, int>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = ingredients.Count,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateMenuItemViewModel model)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
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
                restaurantId, 
                nutritionDict, 
                model.SelectedAllergens,
                model.PrepTime
            );

            var selectedWithQuantities = model.SelectedIngredientIds
                .Where(id => model.IngredientQuantities.ContainsKey(id))
                .Select(id => new { Id = id, Quantity = model.IngredientQuantities[id] })
                .ToList();
            bool result = _menuService.AddIngridientsToMenuItem(
                menuId,
                selectedWithQuantities.Select(x => x.Id).ToArray(),
                selectedWithQuantities.Select(x => x.Quantity).ToArray(),
                restaurantId
            );

            return RedirectToAction("MenuItems");
        }

        [HttpGet]
        public IActionResult MenuItems(string? category)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            List<MenuItem> menuItems;

            if (!string.IsNullOrEmpty(category) && category.ToLower() != "all"
                 && Enum.TryParse<Category>(category, true, out var parsedCategory))
            {
                menuItems = _menuService.GetMenuItemsByCategory(parsedCategory, restaurantId);
            }
            else
            {
                menuItems = _menuService.LoadMenuItemsForManager(restaurantId);
            }

            var viewModels = menuItems.Select(MenuItemViewModel.ConvertToViewModel).ToList();
            ViewBag.SelectedCategory = category;
            return View(viewModels);
        }

        [HttpGet]
        public IActionResult MenuItemDetails(int id)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var menuItem = _menuService.GetMenuItemWithIngredient(id, restaurantId);
            var nutritions = _nutritionService.GetNutritionForMenuItem(id, restaurantId);
            var nutritionVMs = nutritions.Select(NutritionViewModel.ConvertToViewModel).ToList();

            var detailVM = MenuItemDetailViewModel.ConvertToViewModel(menuItem, nutritionVMs);
            return View(detailVM);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var item = _menuService.GetMenuItemWithIngredient(id, restaurantId);
            var availableIngredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);
            var nutritions = _nutritionService.GetNutritionForMenuItem(id, restaurantId);

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
            
            var allergens = _menuService.GetAllergensForMenuItem(id, restaurantId);

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
                    .Select(e => new IngredientViewModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Unit = e.Unit,
                        QuantityInStock = e.QuantityInStock,
                        MinimumStockLevel = e.MinimumStockLevel
                    }).ToList(),
                IngredientQuantities = item.Ingredients
                    .GroupBy(i => i.IngredientId)
                    .ToDictionary(g => g.Key, g => g.First().Quantity),
                NutritionValues = nutritionList,
                SelectedAllergens = allergens,
                PrepTime = item.PrepTime,
                PageNumber = 1,
                PageSize = 5,
                TotalItems = availableIngredients.Count
            };

            ViewData["ButtonClass"] = "edit-page-btn";
            return View("EditMenuItem", model);
        }

        [HttpPost]
        public IActionResult Edit(CreateMenuItemViewModel model)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            if (!ModelState.IsValid)
            {
                RehydrateEditFormData(model);
                return View("EditMenuItem", model);
            }

            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image must be less than 2MB.");
                    RehydrateEditFormData(model);
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
            else
            {
                var existing = _menuService.GetMenuItemWithIngredient(model.Id, restaurantId);
                model.Picture = existing?.Picture ?? "";
            }

            var updated = new MenuItem(
                model.Id,
                model.Name,
                model.Description,
                model.Price,
                model.IsAvailable,
                model.Picture,
                model.Category,
                restaurantId,
                model.PrepTime
            );

            var nutritionDict = model.NutritionValues
                .ToDictionary(e => e.Name, e => e.Value);

            _menuService.UpdateMenuItem(updated, restaurantId);
            _nutritionService.UpdateNutritions(model.Id, nutritionDict, restaurantId);
            _menuService.UpdateMenuItemAllergens(model.Id, model.SelectedAllergens, restaurantId);

            var selectedIngredients = model.IngredientQuantities
                ?.Where(kv => kv.Value > 0)
                .ToDictionary(kv => kv.Key, kv => (decimal)kv.Value)
                ?? new Dictionary<int, decimal>();

            _ingredientService.UpdateMenuItemIngredients(model.Id, selectedIngredients, restaurantId);

            TempData["Success"] = "Menu item updated successfully!";
            return RedirectToAction("MenuItems");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            bool success = _menuService.DeleteMenuItem(id, restaurantId);
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
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
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
                restaurantId: restaurantId 
            );

            _ingredientService.AddIngredient(ingredient);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult IngredientList(int pageNumber = 1, int pageSize = 10)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var ingredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            var pagedIngredients = ingredients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(IngredientViewModel.ConvertToViewModel)
                .ToList();

            var model = new IngredientListViewModel
            {
                Ingredients = pagedIngredients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = ingredients.Count
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetIngredientById(int id)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var ingredient = _ingredientService.GetIngredientById(id, restaurantId);
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
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            var updated = new Ingredient(
                id: model.Id,
                name: model.Name,
                unit: model.Unit,
                quantityInStock: model.QuantityInStock,
                minimumStockLevel: model.MinimumStockLevel,
                restaurantId: restaurantId 
            );

            _ingredientService.UpdateIngredient(updated);
            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult DeleteIngredient(int id)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var ingredient = _ingredientService.GetIngredientById(id, restaurantId);
            if (ingredient == null)
                return NotFound("Ingredient not found.");

            _ingredientService.DeleteIngredient(id, restaurantId);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
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
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
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
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");
            
            var tables = _tableService.GetTablesByRestaurantId(restaurantId);
            return View(tables.Count);
        }

      
        [HttpPost]
        public IActionResult AddNewTable(int tableNumber)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            string guidToken = Guid.NewGuid().ToString();
            string qrUrl = $"{_baseUrl}/Home/LoadingPage?token={guidToken}";
            byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrUrl);

            var table = new Table(qrCodeImage, guidToken, tableNumber);
            _tableService.CreateTableWithNumber(table, restaurantId);

            return RedirectToAction("TableList");
        }

        [HttpGet]
        public IActionResult TableQR(int tableId)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            if (tableId == 0)
            {
                return View("Error"); 
            }

            var qrCode = _tableService.GetTableQRById(tableId, restaurantId);
            var tableNum = _tableService.GetTableNumberById(tableId, restaurantId);

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

        private void RehydrateEditFormData(CreateMenuItemViewModel model)
        {
            var restaurantId = GetCurrentRestaurantId();
            var allIngredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            model.AvailableIngredients = allIngredients
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(i => new IngredientViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Unit = i.Unit,
                    QuantityInStock = i.QuantityInStock,
                    MinimumStockLevel = i.MinimumStockLevel
                }).ToList();

            model.TotalItems = allIngredients.Count;

            var nutritions = _nutritionService.GetNutritionForMenuItem(model.Id, restaurantId);
            model.NutritionValues = Enum.GetValues(typeof(NutritionName))
                .Cast<NutritionName>()
                .Select(n => new NutritionEntry
                {
                    Name = n,
                    Value = nutritions.FirstOrDefault(x => x.Name == n)?.Value ?? 0
                }).ToList();

            model.SelectedAllergens ??= new List<AllergenName>();

            if (string.IsNullOrEmpty(model.Picture))
            {
                var existing = _menuService.GetMenuItemWithIngredient(model.Id, restaurantId);
                model.Picture = existing?.Picture ?? string.Empty;
            }

            ViewData["ButtonClass"] = "edit-page-btn";
        }
    }
}