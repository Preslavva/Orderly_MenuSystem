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
        public void CreateRestaurant(Restaurant restaurant, int ownerId)
        {
            int restaurantId = _restaurantRepository.CreateRestaurant(restaurant);
            _restaurantRepository.AssignRestaurantToOwner(restaurantId, ownerId);   
        }

        public void UpdateRepository(Restaurant restaurant)
        {
            _restaurantRepository.UpdateRestaurant(restaurant);
        }

        public void RemoveRestaurant(int restaurantId)
        {
            _restaurantRepository.RemoveRestaurant(restaurantId);
        }

        public Restaurant GetRestaurantById(int restaurantId)
        {
            return _restaurantRepository.GetRestaurantById(restaurantId);
        }

        public void AssignRestaurantToOwner(int ownerId, int restaurantId)
        {
            _restaurantRepository.AssignRestaurantToOwner(ownerId, restaurantId);
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

        public Restaurant GetRestaurantByKVK(string KVK)
        {
            return _restaurantRepository.GetRestaurantByKVK(KVK);
        }
    }
}
