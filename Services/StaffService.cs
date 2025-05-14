using System;
using System.Collections.Generic;
using System.Linq;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class StaffService
    {
        private readonly StaffRepository _staffRepository;
        private readonly RoleRepository _roleRepository;
        private readonly AuthenticationService _authService;

        public StaffService(StaffRepository staffRepository, RoleRepository roleRepository, AuthenticationService authService)
        {
            _staffRepository = staffRepository;
            _roleRepository = roleRepository;
            _authService = authService;
        }

        public List<Staff> GetAllStaff()
        {
            return _staffRepository.GetAllStaffIncludingInactive();
        }

        public Staff GetStaffById(int id)
        {
            return _staffRepository.GetStaffById(id);
        }

        public bool EmailExists(string email)
        {
            return _staffRepository.GetStaffByEmail(email) != null;
        }

        public (bool success, string message) CreateStaff(string firstName, string lastName, string email, 
            string password, string phone, int roleId, int restaurantId)
        {
            if (EmailExists(email))
            {
                return (false, "A staff member with this email already exists.");
            }

            try
            {
                string salt = _authService.GenerateSalt();
                string hashedPassword = _authService.HashPassword(password, salt);
                var newStaff = new Staff(
                    firstName,
                    lastName,
                    email,
                    phone,
                    true, 
                    restaurantId,
                    hashedPassword,
                    salt
                );
                int staffId = _staffRepository.CreateStaff(newStaff);
                if (staffId > 0 && roleId > 0)
                {
                    _staffRepository.AssignRoleToStaff(staffId, roleId);
                }

                return (true, "Staff member created successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error creating staff member: {ex.Message}");
            }
        }
        
        public (bool success, string message) UpdateStaff(int id, string firstName, string lastName, 
            string email, string phone, bool isActive, int roleId)
        {
            try
            {
                var existingStaff = _staffRepository.GetStaffById(id);
                if (existingStaff == null)
                {
                    return (false, "Staff member not found.");
                }
                if (email != existingStaff.Email)
                {
                    var existingEmail = _staffRepository.GetStaffByEmail(email);
                    if (existingEmail != null && existingEmail.Id != id)
                    {
                        return (false, "A staff member with this email already exists.");
                    }
                }
                
                bool updated = _staffRepository.UpdateStaff(id, firstName, lastName, email, phone, isActive);
                if (!updated)
                {
                    return (false, "Failed to update staff member.");
                }
                
                var existingRoles = _staffRepository.GetStaffRoles(id);
                var currentRoleId = existingRoles.FirstOrDefault()?.Id;

                if (currentRoleId != roleId && roleId > 0)
                {
                    if (currentRoleId.HasValue)
                    {
                        _staffRepository.RemoveRoleFromStaff(id, currentRoleId.Value);
                    }
                    _staffRepository.AssignRoleToStaff(id, roleId);
                }

                return (true, "Staff member updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating staff member: {ex.Message}");
            }
        }
        
        public (bool success, string message) DeleteStaff(int id)
        {
            try
            {
                var existingStaff = _staffRepository.GetStaffById(id);
                if (existingStaff == null)
                {
                    return (false, "Staff member not found.");
                }
                
                bool deleted = _staffRepository.DeleteStaff(id);
                if (!deleted)
                {
                    return (false, "Failed to delete staff member.");
                }

                return (true, "Staff member deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting staff member: {ex.Message}");
            }
        }
    }
}