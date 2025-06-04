using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;
using Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "BusinessAccount");
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





        //Menu Items
        [HttpGet]
        public IActionResult Create(int pageNumber = 1)
        {
            var restaurantId = GetCurrentRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            const int pageSize = 5;
            var ingredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            var pagedIngredients = ingredients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new IngredientViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Unit = i.Unit,
                    QuantityInStock = i.QuantityInStock,
                    MinimumStockLevel = i.MinimumStockLevel
                }).ToList();

            var model = new CreateMenuItemViewModel
            {
                AvailableIngredients = pagedIngredients,
                SelectedIngredientIds = new List<int>(),
                IngredientQuantities = new Dictionary<int, int>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = ingredients.Count
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
                RehydrateCreateFormData(model);
                return View("Create", model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image must be less than 2MB.");
                    return View("Create", model);
                }

                using var image = Image.Load(model.ImageFile.OpenReadStream());

                int cropSize = Math.Min(image.Width, image.Height);
                if (cropSize < 10)
                {
                    ModelState.AddModelError("ImageFile", "Image is too small.");
                    return View("Create", model);
                }

                var cropRect = new Rectangle(
                    (image.Width - cropSize) / 2,
                    (image.Height - cropSize) / 2,
                    cropSize,
                    cropSize
                );

                int targetSize = cropSize / 2;

                image.Mutate(x => x
                    .Crop(cropRect)
                    .Resize(targetSize, targetSize));

                var fileName = $"{Path.GetFileNameWithoutExtension(model.ImageFile.FileName)}_{Guid.NewGuid()}.png";
                var filePath = Path.Combine("wwwroot/images", fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                image.Save(fileStream, new PngEncoder());

                model.Picture = "/images/" + fileName;
            }

            var lowStock = model.SelectedIngredientIds.Any(id =>
            {
                var ingredient = _ingredientService.GetIngredientById(id, restaurantId);
                var requiredQty = model.IngredientQuantities.TryGetValue(id, out var qty) ? qty : 0;
                return ingredient == null || ingredient.QuantityInStock < requiredQty;
            });

            if (model.IsAvailable && lowStock)
            {
                model.IsAvailable = false;
                ModelState.AddModelError("IsAvailable", "Cannot mark as available: some ingredients are understocked.");
                RehydrateCreateFormData(model);
                return View(model);
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

            _menuService.AddIngridientsToMenuItem(
                menuId,
                selectedWithQuantities.Select(x => x.Id).ToArray(),
                selectedWithQuantities.Select(x => x.Quantity).ToArray(),
                restaurantId
            );

            return RedirectToAction("MenuItems");
        }


        [HttpGet]
        public IActionResult MenuItems(string? category, string? availability, bool? lowStockOnly, string? search)
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

            if (!string.IsNullOrEmpty(availability))
            {
                if (availability == "available") menuItems = menuItems.Where(m => m.IsAvailable).ToList();
                else if (availability == "unavailable") menuItems = menuItems.Where(m => !m.IsAvailable).ToList();
            }

            // Low stock filter
            if (lowStockOnly == true)
            {
                menuItems = menuItems.Where(m =>
                    m.Ingredients.Any(i => i.Ingredient.QuantityInStock < i.Quantity)).ToList();
            }

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                menuItems = menuItems.Where(m =>
                    m.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    m.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var viewModels = menuItems.Select(menuItem =>
            {
                var hasLowStock = menuItem.Ingredients != null &&
                                  menuItem.Ingredients.Any(i =>
                                      _ingredientService.GetIngredientById(i.IngredientId, restaurantId).QuantityInStock < i.Quantity);

                return new MenuItemViewModel
                {
                    Id = menuItem.Id,
                    Name = menuItem.Name,
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    Picture = menuItem.Picture,
                    Category = menuItem.Category,
                    IsAvailable = menuItem.IsAvailable,
                    HasLowStockIngredients = hasLowStock
                };
            }).ToList();





            bool isWaiter = TempData["IsWaiter"] != null && (bool)TempData["IsWaiter"];
            ViewBag.IsWaiter = isWaiter;

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
            var item = _menuService.GetMenuItemWithIngredient(id, restaurantId);
            if (item == null) return RedirectToAction("MenuItems");

            var availableIngredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);
            var nutritions = _nutritionService.GetNutritionForMenuItem(id, restaurantId);
            var nutritionList = Enum.GetValues(typeof(NutritionName)).Cast<NutritionName>()
                .Select(n => new NutritionEntry { Name = n, Value = nutritions.FirstOrDefault(x => x.Name == n)?.Value ?? 0 })
                .ToList();

            var allergens = _menuService.GetAllergensForMenuItem(id, restaurantId);
            var selectedQuantities = item.Ingredients
                .GroupBy(i => i.IngredientId)
                .ToDictionary(g => g.Key, g => g.First().Quantity);

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
                    .Select(i => new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Unit = i.Unit,
                        QuantityInStock = i.QuantityInStock,
                        MinimumStockLevel = i.MinimumStockLevel
                    }).ToList(),
                IngredientQuantities = selectedQuantities,
                SelectedIngredientIds = selectedQuantities.Keys.ToList(),
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

            if (!ModelState.IsValid)
            {
                RehydrateEditFormData(model);
                return View("EditMenuItem", model);
            }

            // ✅ Ensure ingredient selection is not null
            model.SelectedIngredientIds ??= model.IngredientQuantities?.Keys.ToList() ?? new List<int>();

            // ✅ Validate that IsAvailable is not allowed with low-stock ingredients
            var lowStock = model.SelectedIngredientIds.Any(id =>
            {
                var ingredient = _ingredientService.GetIngredientById(id, restaurantId);
                var requiredQty = model.IngredientQuantities.TryGetValue(id, out var qty) ? qty : 0;
                return ingredient == null || ingredient.QuantityInStock < requiredQty;
            });

            if (model.IsAvailable && lowStock)
            {
                model.IsAvailable = false;
                ModelState.AddModelError("IsAvailable", "Cannot mark as available: some ingredients are understocked.");
                RehydrateEditFormData(model);
                return View("EditMenuItem", model); // 👈 stay on the form page to show error
            }



            // ✅ Handle image upload
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image must be less than 2MB.");
                    RehydrateEditFormData(model);
                    return View("EditMenuItem", model);
                }

                var uniqueName = $"{Path.GetFileNameWithoutExtension(model.ImageFile.FileName)}_{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
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

            // ❌ Remove this block — it's redundant and overrides valid checkbox input
            // if (model.IsAvailable.Equals(false)) { model.IsAvailable = false; } else { model.IsAvailable = true; }

            // ✅ Create updated MenuItem
            var updated = new MenuItem(
                model.Id, model.Name, model.Description, model.Price,
                model.IsAvailable, model.Picture, model.Category, restaurantId, model.PrepTime);

            _menuService.UpdateMenuItem(updated, restaurantId);
            _nutritionService.UpdateNutritions(model.Id, model.NutritionValues.ToDictionary(e => e.Name, e => e.Value), restaurantId);
            _menuService.UpdateMenuItemAllergens(model.Id, model.SelectedAllergens, restaurantId);

            var selectedIngredients = model.IngredientQuantities?
                .Where(kv => kv.Value > 0)
                .ToDictionary(kv => kv.Key, kv => (decimal)kv.Value)
                ?? new Dictionary<int, decimal>();

            _ingredientService.UpdateMenuItemIngredients(model.Id, selectedIngredients, restaurantId);

            TempData["Success"] = "Menu item updated successfully!";
            return RedirectToAction("MenuItems");
        }

        [HttpGet]
        public IActionResult LoadIngredientsPartialForEdit(int id, int pageNumber = 1, string search = "", string category = "all", bool lowStockOnly = false)
        {
            var restaurantId = GetCurrentRestaurantId();
            const int pageSize = 5;
            var filtered = _ingredientService.GetIngredientsByRestaurantId(restaurantId)
                .Where(i => string.IsNullOrEmpty(search) || i.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Where(i => category == "all" || i.Unit == category)
                .Where(i => !lowStockOnly || i.QuantityInStock < i.MinimumStockLevel)
                .ToList();

            var menuItem = _menuService.GetMenuItemWithIngredient(id, restaurantId);
            var selectedQuantities = menuItem.Ingredients
                .GroupBy(i => i.IngredientId)
                .ToDictionary(g => g.Key, g => g.First().Quantity);

            var pagedIngredients = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new IngredientViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Unit = i.Unit,
                    QuantityInStock = i.QuantityInStock,
                    MinimumStockLevel = i.MinimumStockLevel
                }).ToList();

            var model = new CreateMenuItemViewModel
            {
                Id = menuItem.Id,
                AvailableIngredients = pagedIngredients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = filtered.Count,
                IngredientQuantities = selectedQuantities,
                SelectedIngredientIds = selectedQuantities.Keys.ToList()
            };

            ViewData["ButtonClass"] = "edit-page-btn";
            return PartialView("_IngredientTable", model);

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

        [HttpGet] //dont touch
        public IActionResult LoadIngredientTableForCreate(int pageNumber = 1)
        {
            const int pageSize = 5;
            var restaurantId = GetCurrentRestaurantId();
            var ingredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            var pagedIngredients = ingredients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new IngredientViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Unit = i.Unit,
                    QuantityInStock = i.QuantityInStock,
                    MinimumStockLevel = i.MinimumStockLevel
                }).ToList();

            var model = new CreateMenuItemViewModel
            {
                AvailableIngredients = pagedIngredients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = ingredients.Count,
                SelectedIngredientIds = new List<int>(),
                IngredientQuantities = new Dictionary<int, int>()
            };

            return PartialView("_IngredientTable", model);
        }

        private void RehydrateCreateFormData(CreateMenuItemViewModel model)
        {
            var restaurantId = GetCurrentRestaurantId();
            var allIngredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            // Ensure fallback defaults
            model.PageNumber = model.PageNumber == 0 ? 1 : model.PageNumber;
            model.PageSize = model.PageSize == 0 ? 5 : model.PageSize;

            // Apply pagination
            var pagedIngredients = allIngredients
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToList();

            // Populate available ingredients
            model.AvailableIngredients = pagedIngredients.Select(i => new IngredientViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Unit = i.Unit,
                QuantityInStock = i.QuantityInStock,
                MinimumStockLevel = i.MinimumStockLevel
            }).ToList();

            // Needed for pagination UI
            model.TotalItems = allIngredients.Count;

            // Preserve selections
            model.SelectedIngredientIds ??= new List<int>();
            model.IngredientQuantities ??= new Dictionary<int, int>();
        }






        //Ingredient 
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
        public IActionResult IngredientList()
        {
            int restaurantId = GetCurrentRestaurantId();
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
        private void RehydrateEditFormData(CreateMenuItemViewModel model)
        {
            var restaurantId = GetCurrentRestaurantId();
            var allIngredients = _ingredientService.GetIngredientsByRestaurantId(restaurantId);

            model.PageNumber = model.PageNumber == 0 ? 1 : model.PageNumber;
            model.PageSize = model.PageSize == 0 ? 5 : model.PageSize;

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

            model.NutritionValues ??= new List<NutritionEntry>();
            model.SelectedAllergens ??= new List<AllergenName>();
            model.SelectedIngredientIds ??= new List<int>();
            model.IngredientQuantities ??= new Dictionary<int, int>();

            var nutritions = model.Id > 0
                ? _nutritionService.GetNutritionForMenuItem(model.Id, restaurantId)
                : new List<Nutrition>();

            model.NutritionValues = Enum.GetValues(typeof(NutritionName))
                .Cast<NutritionName>()
                .Select(n => new NutritionEntry
                {
                    Name = n,
                    Value = nutritions.FirstOrDefault(x => x.Name == n)?.Value ?? 0
                }).ToList();

            if (string.IsNullOrEmpty(model.Picture) && model.Id > 0)
            {
                var existing = _menuService.GetMenuItemWithIngredient(model.Id, restaurantId);
                model.Picture = existing?.Picture ?? string.Empty;
            }

            ViewData["ButtonClass"] = "edit-page-btn";
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








        //Tables
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

            if (qrCode == null || tableNum == null)
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