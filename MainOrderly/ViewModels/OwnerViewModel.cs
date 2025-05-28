using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class OwnerViewModel
    {
        public int Id { get; set; }

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

        public string FullName => $"{FirstName} {LastName}";

        public static OwnerViewModel FromEntity(Owner owner)
        {
            return new OwnerViewModel
            {
                Id = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Email = owner.Email,
                Phone = owner.Phone
            };
        }
    }
}