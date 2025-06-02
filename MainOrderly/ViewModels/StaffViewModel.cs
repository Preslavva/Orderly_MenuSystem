using System.ComponentModel.DataAnnotations;

namespace MainOrderly.WebApp.ViewModels
{
    public class StaffViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RestaurantId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        
        public string FullName => $"{FirstName} {LastName}";
        public string RolesDisplay => string.Join(", ", Roles);
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
    }
}