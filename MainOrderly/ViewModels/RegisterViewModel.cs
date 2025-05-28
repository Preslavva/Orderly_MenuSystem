using System.ComponentModel.DataAnnotations;

namespace MainOrderly.WebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Restaurant email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Restaurant Email")]
        public string RestaurantEmail { get; set; }

        [Required(ErrorMessage = "Restaurant phone is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Restaurant Phone")]
        public string RestaurantPhone { get; set; }

        [Required(ErrorMessage = "Restaurant address is required")]
        [Display(Name = "Restaurant Address")]
        public string RestaurantAddress { get; set; }

        [Required(ErrorMessage = "KVK number is required")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "KVK number must be exactly 8 digits")]
        [Display(Name = "KVK Number")]
        public string KVK { get; set; }

        [Display(Name = "Restaurant Description")]
        public string Description { get; set; }
    }

}