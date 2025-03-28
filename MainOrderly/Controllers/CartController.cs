using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.ViewModels;

namespace MainOrderly.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        private Dictionary<MenuItem, int> GetOrderList()
        {
            return _cartService.GetCart();
        }

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Converts the Dictionary in Cart into View Model
        /// </summary>
        /// <returns>List with CatViewModels</returns>
        private List<CartViewModel> GetCartModel()
        {
            List<CartViewModel> model = GetOrderList().Select(item => CartViewModel.ConvertToViewModel(item.Key, item.Value)).ToList();
            return model;
        }

        [HttpGet]
        public IActionResult OrderList()
        {
            ViewData["Page"] = "Order overview";
            List<CartViewModel> model = GetCartModel();
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
            List<CartViewModel> model = GetCartModel();
            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(GetOrderList());
            return View(model);
        }

        [HttpPost]
        public IActionResult GoToOrderSummary()
        {
            return RedirectToAction("OrderSummaryPage", "Cart");
        }

        [HttpGet]
        public IActionResult PaymentConfirmationPage()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            int tableId = HttpContext.Session.GetInt32("TableId") ?? 0;
            List<CartViewModel> model = GetCartModel();
            if (model.Count == 0)
            {
                return RedirectToAction("OrderList");
            }

            int newOrderId = _cartService.FinalizeOrder(tableId);

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
    }
}
