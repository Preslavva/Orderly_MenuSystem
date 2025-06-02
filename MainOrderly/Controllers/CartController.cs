using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using Models.Enums;
using MainOrderly.WebApp.Extensions;

namespace MainOrderly.WebApp.Controllers
{
    public class CartController : BaseController
    {
        private readonly CartService _cartService;
        private readonly IngredientService _ingredientService;
        private readonly HistoryService _historyService;
        private readonly RestaurantService _restaurantService;

        public CartController(CartService cartService, IngredientService ingredientService, 
            HistoryService historyService, RestaurantService restaurantService): base(restaurantService)
        {
            _cartService = cartService;
            _ingredientService = ingredientService;
            _historyService = historyService;
            _restaurantService = restaurantService;
        }

        private int GetRestaurantId()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            return user?.RestaurantId ?? 1;
        }

        private Dictionary<MenuItem, int> GetOrderList()
        {
            int restaurantId = GetRestaurantId();
            return _cartService.GetCart(restaurantId); 
        }

        private List<CartViewModel> GetCartViewModel()
        {
            List<CartViewModel> viewModel = GetOrderList().Select(item => CartViewModel.ConvertToViewModel(item.Key, item.Value)).ToList();
            return viewModel;
        }

        [HttpGet]
        public IActionResult OrderList()
        {
            ViewData["Page"] = "Order overview";
            List<CartViewModel> model = GetCartViewModel();

            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(GetOrderList());
            TempData["CartCount"] = _cartService.GetCartCount();
            return View(model);
        }
            
        [HttpPost]
        public IActionResult RemoveItemFromCart(int id)
        {
            int restaurantId = GetRestaurantId();
            _cartService.RemoveFromCart(id);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateItemQuantity(int id, int quantity)
        {
            int restaurantId = GetRestaurantId();
            _cartService.UpdateQuantity(id, quantity);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpGet]
        public IActionResult OrderSummaryPage(Tip tipAmount, int customTip)    
        {
            ViewData["Page"] = "Order summary";
            List<CartViewModel> viewModel = GetCartViewModel();

            ViewBag.SelectedTip = tipAmount;
            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(GetOrderList(), tipAmount, customTip);
            ViewBag.NoTipTotalPrice = _cartService.CalculateTotalPrice(GetOrderList());
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GoToOrderSummary()
        {
            return RedirectToAction("OrderSummaryPage", "Cart");
        }

        [HttpGet]
        public IActionResult PaymentConfirmationPage(int orderId)
        {
            ViewData["Page"] = "Order Confirmation Page";
            OrderViewModel orderModel = new OrderViewModel()
            {
                Id = orderId
            };

            return View(orderModel);
        }

        public IActionResult Checkout()
        {
            int tableId = HttpContext.Session.GetInt32("TableId") ?? 0;
            int restaurantId = GetRestaurantId();

            Restaurant restaurant = _restaurantService.GetRestaurantById(restaurantId);
            if (restaurant == null)
            {
                TempData["ErrorMessage"] = "Restaurant not found.";
                return RedirectToAction("OrderSummaryPage", "Cart");
            }

            int? oldOrderId = HttpContext.Session.GetInt32("oldOrderId");
            if (oldOrderId != null)
            {
                bool IsNotExpired = _cartService.CheckTimeBetweenOrders(oldOrderId, restaurantId);
                if (!IsNotExpired)
                {
                    TempData["ErrorMessage"] = "You cannot place another order because the time restriction has not expired.";
                    return RedirectToAction("OrderSummaryPage", "Cart");
                }
            }

            List<CartViewModel> viewModel = GetCartViewModel();
            if (viewModel.Count == 0)
            {
                return RedirectToAction("OrderList");
            }
            int newOrderId = _cartService.FinalizeOrder(tableId, restaurant, restaurantId);
            
            _historyService.SaveOrderIds(newOrderId);
            HttpContext.Session.SetInt32("oldOrderId", newOrderId);

            _cartService.SaveCart(new Dictionary<int, int>());
            _cartService.ClearCart();
            
            return RedirectToAction("PaymentConfirmationPage", new { orderId = newOrderId });
        }

        [HttpPost]
        public IActionResult GoToPaymentConfirmation()
        {
            return RedirectToAction("PaymentConfirmationPage", "Cart");
        }

        [HttpPost]
        public IActionResult GetReceipt()
        {
            TempData["Receipt"] = "The receipt was sent to your email!";
            return RedirectToAction("PaymentConfirmationPage", "Cart");
        }

        public IActionResult Timer(int orderId)
        {
            int restaurantId = GetRestaurantId();
            DateTime endOfTimer = _cartService.GetEndOfTimer(orderId, restaurantId);  
            TimeSpan remainingTime = endOfTimer - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                return Content("Time's up");
            }

            string formattedTime = $"{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            return Content(formattedTime);
        }
    }
}
