using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class RestaurantViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
		public string Logo { get; set; }
        public IFormFile logoImage { get; set; }    

		[Required]
		public string Email { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Address { get; set; }
        public string Font { get; set; }
        public string ColorButtons { get; set; }
        public string ColorDeafult { get; set; }
        public string ColorBackground { get; set; }

		[Required]
		public string KVK { get; set; }
        public List<SelectListItem> Fonts { get; set; } 

        public static RestaurantViewModel ConvertToViewModel(Restaurant restaurant)
        {
            RestaurantViewModel viewModel = new RestaurantViewModel()
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Logo = Convert.ToBase64String(restaurant.Logo),
                Email = restaurant.Email,
                PhoneNumber = restaurant.PhoneNumber,
                Address = restaurant.Address,
                Font = restaurant.Font,
                ColorButtons = restaurant.ColorButtons,
                ColorDeafult = restaurant.ColorDefault,
                ColorBackground = restaurant.ColorBackground,
                KVK = restaurant.KVK,
                Fonts = new List<SelectListItem>()
            };
            return viewModel;
        }

        public static Restaurant ConvertToEntity(RestaurantViewModel model)
        { 
            return new Restaurant(model.Id, model.Name, model.Email, model.PhoneNumber, model.Address, model.Description, Convert.FromBase64String(model.Logo), model.Font, model.ColorButtons, model.ColorDeafult, model.ColorBackground, model.KVK);
        }

    }
}
