using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;

namespace MainOrderly.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartServices _cartServices;

        public CartController(CartServices cartServices)
        {
            _cartServices = cartServices;
        }

        private Dictionary<MenuItem, int> GetOrderList()
        {
            return _cartServices.GetCart();
        }

        [HttpGet]
        public IActionResult OrderList()
        {
            ViewData["Page"] = "Order overview";
            Dictionary<MenuItem, int> cart = GetOrderList();
            ViewBag.TotalPrice = _cartServices.CalculateTotalPrice(cart);
            TempData["CartCount"] = _cartServices.GetCartCount();
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveItemFromCart(int id)
        {
            _cartServices.RemoveFromCart(id);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateItemQuantity(int id, int quantity)
        {
            _cartServices.UpdateQuantity(id, quantity);
            return RedirectToAction("OrderList", "Cart");
        }

        [HttpGet]
        public IActionResult OrderSummaryPage()
        {
            ViewData["Page"] = "Order summary";
            Dictionary<MenuItem, int> cart = GetOrderList();
            ViewBag.TotalPrice = _cartServices.CalculateTotalPrice(cart);
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
            Dictionary<MenuItem, int> cart = _cartServices.GetCart();

            if (cart.Count == 0)
            {
                return RedirectToAction("OrderList");
            }

            int newOrderId = _cartServices.FinalizeOrder(tableId);

            _cartServices.SaveCart(new Dictionary<int, int>());

            _cartServices.ClearCart();
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
