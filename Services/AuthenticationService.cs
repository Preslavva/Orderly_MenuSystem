using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class AuthenticationService
    {
        private readonly OwnerRepository _ownerRepository;
        private readonly StaffRepository _staffRepository;
        private readonly RoleRepository _roleRepository;

        public AuthenticationService(OwnerRepository ownerRepository, StaffRepository staffRepository, RoleRepository roleRepository)
        {
            _ownerRepository = ownerRepository;
            _staffRepository = staffRepository;
            _roleRepository = roleRepository;
        }

        public AuthenticatedUser AuthenticateUser(string email, string password)
        {
            Owner owner = _ownerRepository.GetOwnerByEmail(email);
            if (owner != null)
            {
                string hashedPassword = HashPassword(password, owner.Salt);
                if (hashedPassword == owner.Password)
                {
                    return new AuthenticatedUser
                    {
                        Id = owner.Id,
                        Email = owner.Email,
                        FullName = owner.FullName,
                        UserType = "Owner", 
                        RestaurantId = owner.RestaurantId,
                        Roles = new List<string> { "Owner" }
                    };
                }
            }
            Staff staff = _staffRepository.GetStaffByEmail(email);
            if (staff != null && staff.IsActive)
            {
                string hashedPassword = HashPassword(password, staff.Salt);
                if (hashedPassword == staff.Password)
                {
                    List<Role> staffRoles = _staffRepository.GetStaffRoles(staff.Id);
                    staff.SetRoles(staffRoles);

                    string primaryRole = staffRoles.Count > 0 ? staffRoles[0].Type : "Staff";

                    return new AuthenticatedUser
                    {
                        Id = staff.Id,
                        Email = staff.Email,
                        FullName = staff.FullName,
                        UserType = primaryRole, 
                        RestaurantId = staff.RestaurantId,
                        Roles = staffRoles.ConvertAll(r => r.Type)
                    };
                }
            }

            return null; 
        }

        public string HashPassword(string password, string salt)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string combinedString = password + salt;
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenerateSalt(int size = 24)
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[size];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
    }

    public class AuthenticatedUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; } 
        public int RestaurantId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}