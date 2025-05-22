using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;
        public RestaurantService(RestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
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
    }
}
