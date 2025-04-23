using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;
using System.Runtime.CompilerServices;

namespace MainOrderly.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly HistoryService _historyService;

        private Dictionary<MenuItem, int> GetOrderList()
        {
            return _cartService.GetCart();
        }

        public CartController(CartService cartService, HistoryService historyService)
        {
            _cartService = cartService;
            _historyService = historyService;
        }

        /// <summary>
        /// Converts the Dictionary in Cart into View Model
        /// </summary>
        /// <returns>List with CatViewModels</returns>
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
            _cartService.RemoveFromCart(id);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateItemQuantity(int id, int quantity)
        {
            _cartService.UpdateQuantity(id, quantity);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpGet]
        public IActionResult OrderSummaryPage()
        {
            ViewData["Page"] = "Order summary";
            List<CartViewModel> viewModel = GetCartViewModel();
            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(GetOrderList());
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
            ViewData["Page"] = "Payment";
            OrderViewModel orderModel = new OrderViewModel()
            {
                Id = orderId
            };

            if(!_cartService.CheckTimeBetweenOrders(orderId))
            {
                Response.Cookies.Append("order-restricted", "true", new CookieOptions
                {
                    IsEssential = true, 
                    Expires = DateTime.Now.AddMinutes(5) 
                });
            }

            return View(orderModel);
        }

        public IActionResult Checkout()
        {
            int tableId = HttpContext.Session.GetInt32("TableId") ?? 0;
            // Create a hardcoded test restaurant
            int restaurantId = 1; // Use a default ID
            byte[] emptyLogo = new byte[0]; // Empty byte array for logo

            // Create a simple restaurant object
            Restaurant testRestaurant = new Restaurant(
                restaurantId,
                "Test Restaurant",
                "This is a test restaurant for checkout",
                emptyLogo,
                "Blue",
                "test@example.com",
                "123-456-7890",
                "123 Test Street"
            );

            List<CartViewModel> viewModel = GetCartViewModel();
            if (viewModel.Count == 0)
            {
                return RedirectToAction("OrderList");
            }

            int newOrderId = _cartService.FinalizeOrder(tableId, testRestaurant);
            _historyService.SaveOrderIds(newOrderId);

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
            TimeSpan endOfTimer = _cartService.GetEndOfTimer(orderId);  
            TimeSpan remainingTime = endOfTimer - DateTime.Now.TimeOfDay;

            if (remainingTime.TotalSeconds <= 0)
            {
                Response.Cookies.Delete("order-restricted");
                return Content(null);
            }
            else
            {
                Response.Cookies.Append("order-restricted", "true", new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTime.Now.AddMinutes(5)
                });
            }


            string countdownHtml = $"<div id='countdown' class='AntiAbuseTimer'  data-order-id='{orderId}'>" +
                                   $"{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}</div>";

            return Content(countdownHtml, "text/html");
        }

    }
}