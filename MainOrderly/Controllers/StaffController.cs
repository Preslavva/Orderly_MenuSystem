using MainOrderly.WebApp.Attributes;
using MainOrderly.WebApp.Extensions;
using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSSQL;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainOrderly.WebApp.Controllers
{
    [RequireRole("Owner")]
    public class StaffController : Controller
    {
        private readonly StaffService _staffService;
        private readonly RoleRepository _roleRepository;
        private readonly StaffRepository _staffRepository;  

        public StaffController(StaffService staffService, RoleRepository roleRepository, StaffRepository staffRepository)
        {
            _staffService = staffService;
            _roleRepository = roleRepository;
            _staffRepository = staffRepository; 
        }

        public IActionResult AllStaff()
        {
            var staff = _staffRepository.GetAllStaffIncludingInactive();
            var viewModels = staff.Select(s => new StaffViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Phone = s.Phone,
                IsActive = s.IsActive
            }).ToList();
            return View("~/Views/Business/AllStaff.cshtml", viewModels);
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            var roles = _roleRepository.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Type");
            return View("~/Views/Business/AddStaff.cshtml", new AddStaffViewModel());
        }

        [HttpPost]
        public IActionResult AddStaff(AddStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = _roleRepository.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Type");
                
                return View("~/Views/Business/AddStaff.cshtml", model);
            }
            var currentUser = HttpContext.Session.GetAuthenticatedUser();
            int restaurantId = currentUser?.RestaurantId ?? 1; 

            var (success, message) = _staffService.CreateStaff(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.Phone,
                model.RoleId,
                restaurantId
            );

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("AllStaff");
            }
            else
            {
                ModelState.AddModelError("", message);
                var roles = _roleRepository.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Type");
                return View("~/Views/Business/AddStaff.cshtml", model);
            }
        }
        
        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            var staff = _staffService.GetStaffById(id);
            if (staff == null)
            {
                return NotFound();
            }

            var roles = _roleRepository.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Type");

            var staffRoles = _staffRepository.GetStaffRoles(id);
            int currentRoleId = staffRoles.FirstOrDefault()?.Id ?? 0;

            var model = new EditStaffViewModel
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                Phone = staff.Phone,
                IsActive = staff.IsActive,
                RoleId = currentRoleId
            };
            return View("~/Views/Business/EditStaff.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditStaff(EditStaffViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var roles = _roleRepository.GetAllRoles();
                    ViewBag.Roles = new SelectList(roles, "Id", "Type");
                    return View("~/Views/Business/EditStaff.cshtml", model);
                }

                var (success, message) = _staffService.UpdateStaff(
                    model.Id,
                    model.FirstName,
                    model.LastName, 
                    model.Email,
                    model.Phone,
                    model.IsActive,
                    model.RoleId
                );

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction("AllStaff");
                }
                else
                {
                    ModelState.AddModelError("", message);
                    var roles = _roleRepository.GetAllRoles();
                    ViewBag.Roles = new SelectList(roles, "Id", "Type");
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
            var (success, message) = _staffService.DeleteStaff(id);
    
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }
    
            return RedirectToAction("AllStaff");
        }
    }
}