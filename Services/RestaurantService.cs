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
            ValidateRestaurant(restaurant);
            int restaurantId = _restaurantRepository.CreateRestaurant(restaurant);
            _restaurantRepository.AssignRestaurantToOwner(ownerId, restaurantId);   
        }

        public void UpdateRepository(Restaurant restaurant)
        {
            ValidateRestaurant(restaurant);
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

        public Restaurant GetOwnerRestaurant(int? ownerId)
        {
            return _restaurantRepository.GetOwnerRestaurant(ownerId);
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

        private void ValidateRestaurant(Restaurant restaurant)
        {
            if (restaurant.Logo == null)
                throw new ArgumentException("Please add a logo!");

            if (restaurant.Name == null)
                throw new ArgumentException("Please enter a name!");

            if(restaurant.Description == null)
                throw new ArgumentException("Please enter a description!");

            if (restaurant.Address == null)
                throw new ArgumentException("Please enter an address!");

            if (restaurant.Email == null)
                throw new ArgumentException("Please enter an email!");

            if (restaurant.PhoneNumber == null)
                throw new ArgumentException("Please enter a phone number!");

            if (restaurant.KVK == null)
                throw new ArgumentException("Please enter a Kvk number!");

            if (_restaurantRepository.DoesKvkExist(restaurant))
                throw new ArgumentException("Kvk number should be unique!");
        }
    }
}
