using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;

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


        [HttpGet]
        public IActionResult OrderList()
        {
            ViewData["Page"] = "Order overview";
            Dictionary<MenuItem, int> cart = GetOrderList();
            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(cart);
            TempData["CartCount"] = _cartService.GetCartCount();
            return View(cart);
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
            Dictionary<MenuItem, int> cart = GetOrderList();
            ViewBag.TotalPrice = _cartService.CalculateTotalPrice(cart);
            return View(cart);
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
            Dictionary<MenuItem, int> cart = _cartService.GetCart();

            if (cart.Count == 0)
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
