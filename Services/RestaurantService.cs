using Microsoft.AspNetCore.Http;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly OwnerRepository _ownerRepository;
        public RestaurantService(RestaurantRepository restaurantRepository, OwnerRepository ownerRepository)
        {
            _restaurantRepository = restaurantRepository;
            _ownerRepository = ownerRepository;
        }
        public void CreateRestaurant(Restaurant restaurant)
        {
            _restaurantRepository.CreateRestaurant(restaurant);
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
            _ownerRepository.AssignRestaurantToOwner(ownerId, restaurantId);
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
