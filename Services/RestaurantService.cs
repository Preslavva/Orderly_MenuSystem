using Microsoft.AspNetCore.Http;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly StaffRepository _staffRepository;
        public RestaurantService(RestaurantRepository restaurantRepository, StaffRepository staffRepository)
        {
            _restaurantRepository = restaurantRepository;
            _staffRepository = staffRepository;
        }

        public void CustomizeRestaurant(Restaurant restaurant)
        {
            _restaurantRepository.CustomizeRestaurant(restaurant);
        }   
        
        public Restaurant GetRestaurantById(int restaurantId)
        {
            return _restaurantRepository.GetRestaurantById(restaurantId);
        }

        public Restaurant GetOwnerRestaurant(int? ownerId)
        {
            return _restaurantRepository.GetOwnerRestaurant(ownerId);
        }

		public string ConvertToString(IFormFile image)
		{
			using (var ms = new MemoryStream())
			{
				image.CopyTo(ms);
				byte[] imageBytes = ms.ToArray();
				return Convert.ToBase64String(imageBytes);
			}
		}
    }
}
