using MainOrderly.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        public RestaurantController(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService; 
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            if(id != 0)
            {
                Restaurant restaurant = _restaurantService.GetRestaurantById(id);
                RestaurantViewModel restaurantModel = RestaurantViewModel.ConvertToViewModel(restaurant);
                return View(restaurant);    
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateRestaurant(RestaurantViewModel restaurantModel, int ownerId)
        {
            Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantModel);
            _restaurantService.CreateRestaurant(restaurant);
            //_restaurantService.AssignRestaurantToOwner(ownerId,);
            return RedirectToAction();
        }

        [HttpPost]
        public IActionResult UpdateRestaurant(RestaurantViewModel restaurantViewModel)
        {
            Restaurant restaurant = RestaurantViewModel.ConvertToEntity(restaurantViewModel);
            _restaurantService.UpdateRepository(restaurant);
            return RedirectToAction();
        }

        [HttpPost]
        public IActionResult ArchiveRestaurant(int id)
        {
            _restaurantService.RemoveRestaurant(id);
            return RedirectToAction();
        }

        [HttpPost]
        public IActionResult LoadOwnerRestaurant(int id)
        {
            _restaurantService.GetRestaurantById(id);
            return RedirectToAction();
        }
    }
}
