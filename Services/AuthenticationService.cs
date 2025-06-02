using System.Security.Cryptography;
using System.Text;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class AuthenticatedUser
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; } // "Owner" or "Staff"
        public int RestaurantId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class AuthenticationService
    {
        private readonly OwnerRepository _ownerRepository;
        private readonly StaffRepository _staffRepository;

        public AuthenticationService(OwnerRepository ownerRepository, StaffRepository staffRepository)
        {
            _ownerRepository = ownerRepository;
            _staffRepository = staffRepository;
        }

        public AuthenticatedUser Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;
            var owner = _ownerRepository.GetByEmail(email);
            if (owner != null && VerifyPassword(password, owner.Password, owner.Salt))
            {
                return new AuthenticatedUser
                {
                    UserId = owner.Id,
                    FullName = owner.FullName,
                    Email = owner.Email,
                    UserType = "Owner",
                    RestaurantId = owner.RestaurantId,
                    Roles = new List<string> { "Owner" }
                };
            }
            var staff = _staffRepository.GetStaffByEmailGlobal(email);
            if (staff != null && staff.IsActive && VerifyPassword(password, staff.Password, staff.Salt))
            {
                var roles = _staffRepository.GetStaffRoles(staff.Id, staff.RestaurantId);
                return new AuthenticatedUser
                {
                    UserId = staff.Id,
                    FullName = staff.FullName,
                    Email = staff.Email,
                    UserType = "Staff",
                    RestaurantId = staff.RestaurantId,
                    Roles = roles.Select(r => r.Type).ToList()
                };
            }

            return null;
        }

        public bool RegisterOwner(string firstName, string lastName, string email, string phone, 
            string password, string restaurantName, string restaurantEmail, string restaurantPhone, 
            string restaurantAddress, string kvk, string description = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(restaurantName) || string.IsNullOrWhiteSpace(kvk))
                {
                    return false;
                }
                if (_ownerRepository.GetByEmail(email) != null || _staffRepository.GetStaffByEmailGlobal(email) != null)
                {
                    return false;
                }

                if (_ownerRepository.IsKvkExists(kvk))
                {
                    return false;
                }

                var salt = GenerateSalt();
                var hashedPassword = HashPassword(password, salt);

                return _ownerRepository.RegisterOwnerWithRestaurant(
                    firstName, lastName, email, phone, hashedPassword, salt,
                    restaurantName, restaurantEmail, restaurantPhone, restaurantAddress, kvk, description);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateStaff(string firstName, string lastName, string email, string phone, 
            string password, int restaurantId, List<int> roleIds = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }

                if (_ownerRepository.GetByEmail(email) != null || _staffRepository.GetStaffByEmailGlobal(email) != null)
                {
                    return false;
                }

                var salt = GenerateSalt();
                var hashedPassword = HashPassword(password, salt);

                var staff = new Staff(firstName, lastName, email, phone, true, restaurantId, hashedPassword, salt);
                var staffId = _staffRepository.CreateStaff(staff);

                if (roleIds != null && roleIds.Any())
                {
                    foreach (var roleId in roleIds)
                    {
                        _staffRepository.AssignRoleToStaff(staffId, roleId, restaurantId);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(salt))
                return false;

            var hash = HashPassword(password, salt);
            return hash == hashedPassword;
        }

        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }

        public string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public bool ChangePassword(int userId, string userType, string currentPassword, string newPassword)
        {
            try
            {
                if (userType == "Owner")
                {
                    var owner = _ownerRepository.GetById(userId);
                    if (owner != null && VerifyPassword(currentPassword, owner.Password, owner.Salt))
                    {
                        var newSalt = GenerateSalt();
                        var newHash = HashPassword(newPassword, newSalt);
                        return true; 
                    }
                }
                else if (userType == "Staff")
                {
                    return true; 
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}