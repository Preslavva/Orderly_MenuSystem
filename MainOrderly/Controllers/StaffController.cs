using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSSQL;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner")]
    public class StaffController : Controller
    {
        private readonly StaffService _staffService;
        private readonly RoleRepository _roleRepository;
        private readonly StaffRepository _staffRepository;
        private readonly AuthenticationService _authService;

        public StaffController(StaffService staffService, RoleRepository roleRepository, 
            StaffRepository staffRepository, AuthenticationService authService)
        {
            _staffService = staffService;
            _roleRepository = roleRepository;
            _staffRepository = staffRepository;
            _authService = authService;
        }

        private int GetRestaurantId()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            return user?.RestaurantId ?? 0;
        }

        public IActionResult AllStaff()
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            var staff = _staffRepository.GetAllStaffIncludingInactive(restaurantId);
            var viewModels = new List<StaffViewModel>();
            foreach (var s in staff)
            {
                viewModels.Add(new StaffViewModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Phone = s.Phone,
                    IsActive = s.IsActive,
                    RestaurantId = s.RestaurantId,
                    Roles = _staffRepository.GetStaffRoles(s.Id, restaurantId).Select(r => r.Type).ToList()
                });
            }

            return View("~/Views/Business/AllStaff.cshtml", viewModels);
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            var roles = _roleRepository.GetAllRoles(restaurantId);
            ViewBag.Roles = new SelectList(roles, "Id", "Type");
            return View("~/Views/Business/AddStaff.cshtml", new CreateStaffViewModel());
        }

        [HttpPost]
        public IActionResult AddStaff(CreateStaffViewModel model)
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            if (!ModelState.IsValid)
            {
                var roles = _roleRepository.GetAllRoles(restaurantId);
                ViewBag.Roles = new SelectList(roles, "Id", "Type");
                return View("~/Views/Business/AddStaff.cshtml", model);
            }

            var success = _authService.CreateStaff(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Phone,
                model.Password,
                restaurantId,
                model.SelectedRoleIds
            );

            if (success)
            {
                TempData["SuccessMessage"] = "Staff member created successfully.";
                return RedirectToAction("AllStaff");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create staff member. Email may already exist.");
                var roles = _roleRepository.GetAllRoles(restaurantId);
                ViewBag.Roles = new SelectList(roles, "Id", "Type");
                return View("~/Views/Business/AddStaff.cshtml", model);
            }
        }
        
        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            var staff = _staffRepository.GetStaffById(id, restaurantId);
            if (staff == null)
            {
                return NotFound();
            }

            var roles = _roleRepository.GetAllRoles(restaurantId);
            ViewBag.Roles = new SelectList(roles, "Id", "Type");

            var staffRoles = _staffRepository.GetStaffRoles(id, restaurantId);
            var currentRoleId = staffRoles.FirstOrDefault()?.Id ?? 0;

            var model = new EditStaffViewModel
            {
                Id = id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                Phone = staff.Phone,
                IsActive = staff.IsActive,
                RoleId = currentRoleId 
            };

            ViewBag.StaffId = id;
            return View("~/Views/Business/EditStaff.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditStaff(int id, EditStaffViewModel model)
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            try
            {
                if (!ModelState.IsValid)
                {
                    var roles = _roleRepository.GetAllRoles(restaurantId);
                    ViewBag.Roles = new SelectList(roles, "Id", "Type");
                    ViewBag.StaffId = id;
                    return View("~/Views/Business/EditStaff.cshtml", model);
                }

                var success = _staffRepository.UpdateStaff(
                    id,
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Phone,
                    model.IsActive,
                    restaurantId
                );

                if (success)
                {
                    var currentRoles = _staffRepository.GetStaffRoles(id, restaurantId);
                    var currentRoleIds = currentRoles.Select(r => r.Id).ToList();

                    foreach (var roleId in currentRoleIds)
                    {
                        _staffRepository.RemoveRoleFromStaff(id, roleId, restaurantId);
                    }

                    if (model.RoleId > 0)
                    {
                        _staffRepository.AssignRoleToStaff(id, model.RoleId, restaurantId);
                    }

                    TempData["SuccessMessage"] = "Staff member updated successfully.";
                    return RedirectToAction("AllStaff");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update staff member.");
                    var roles = _roleRepository.GetAllRoles(restaurantId);
                    ViewBag.Roles = new SelectList(roles, "Id", "Type");
                    ViewBag.StaffId = id;
                    return View("~/Views/Business/EditStaff.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("AllStaff");
            }
        }
        
        [HttpPost]
        public IActionResult DeleteStaff(int id)
        {
            var restaurantId = GetRestaurantId();
            if (restaurantId == 0) return RedirectToAction("Login", "BusinessAccount");

            try
            {
                var success = _staffRepository.DeleteStaff(id, restaurantId);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Staff member deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete staff member.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting staff member: {ex.Message}";
            }
    
            return RedirectToAction("AllStaff");
        }
    }
}