using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using MSSQL;
using System.Collections.Generic;
using System.Text.Json;
using Models.Enums;

namespace Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly MenuItemRepository _menuItemRepository;
        private Dictionary<int, int> cart;
        private readonly CartRepository _cartRepository;
        private readonly CheckoutService _checkoutService;
        private const int timer = 5;
        private readonly IngredientService _ingredientService;
        private readonly IngredientRepository _ingredientRepository;


        public CartService(IHttpContextAccessor contxtAccessor, MenuItemRepository menuItemRepository, CartRepository cartRepository, CheckoutService checkoutService, IngredientService ingredientService,IngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
            _ingredientService = ingredientService;
            _checkoutService = checkoutService;
            _cartRepository = cartRepository;
            contextAccessor = contxtAccessor;
            _menuItemRepository = menuItemRepository;
            cart = new Dictionary<int, int>();

            string? jsonCart = contextAccessor.HttpContext?.Request.Cookies["Cart"];
            if (!string.IsNullOrEmpty(jsonCart))
            {
                cart = JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCart)!;
            }

            _ingredientService = ingredientService;
        }

        public void AddToCart(int id, int quantity)
        {
            if (cart.ContainsKey(id))
            {
                cart[id] += quantity;
            }
            else
            {
                cart[id] = quantity;
            }
            SaveCart();
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (cart.ContainsKey(id))
            {
                cart[id] = newQuantity;
            }
            SaveCart();
        }

        public void RemoveFromCart(int id)
        {
            if (cart.ContainsKey(id))
            {
                cart.Remove(id);
            }
            SaveCart();
        }

        public void SaveCart()
        {
            string jsonCart = JsonSerializer.Serialize(cart);
            //contextAccessor.HttpContext?.Session.SetString("Cart", jsonCart);

            contextAccessor.HttpContext?.Response.Cookies.Append("Cart", jsonCart, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });
        }
        // Overload that accepts a dictionary
        public void SaveCart(Dictionary<int, int> newCart)
        {
            cart = newCart;
            SaveCart();
        }
        public void ClearCart()
        {
            // Another convenience method to empty the cart
            cart = new Dictionary<int, int>();
            SaveCart();
        }

        public int GetCartCount()
        {
            int counter = 0;
            foreach (int quantity in cart.Values)
            {
                counter += quantity;
            }
            return counter;
        }
        int restauranrId = 1;
        public Dictionary<MenuItem, int> GetCart()
        {
            Dictionary<MenuItem, int> newCart = new Dictionary<MenuItem, int>();

            string? jsonCart = contextAccessor.HttpContext?.Request.Cookies["Cart"];
            if (!string.IsNullOrEmpty(jsonCart))
            {
                cart = JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCart)!;
                foreach (int key in cart.Keys)
                {
                    MenuItem? item = _menuItemRepository.GetMenuItemById(key, restauranrId);

                    newCart[item] = cart[key];
                }
            }
            return newCart;
        }

        public decimal CalculateTotalPrice(Dictionary<MenuItem, int> cart, Tip tipAmount,int customTip)
        {
            if (customTip >= 100)
                customTip = 100;

            decimal totalPrice = 0;
            foreach (var element in cart)
            {
                MenuItem item = element.Key;
                int quantity = element.Value;
                totalPrice += quantity * item.Price;
            }

            if(tipAmount == Tip.Tip15)
            {
                totalPrice += totalPrice * 0.15m;
            }
            else if (tipAmount == Tip.Tip25)
            {
                totalPrice += totalPrice * 0.25m;
            }
            else if (customTip>0)
                totalPrice += totalPrice * (customTip / 100m);
            
            return totalPrice;
        }
        public decimal CalculateTotalPrice(Dictionary<MenuItem, int> cart) // overloading method, for no tipping.
        {
            decimal totalPrice = 0;
            foreach (var element in cart)
            {
                MenuItem item = element.Key;
                int quantity = element.Value;
                totalPrice += quantity * item.Price;
            }
            
            return totalPrice;
        }

        public int FinalizeOrder(int tableId, Restaurant restaurant)
        {

            tableId = 6; // test, to remove later
            Dictionary<MenuItem, int> cart = GetCart();

            foreach(var element in cart)
            {
                var ingredient = _ingredientRepository.GetIngredientsForMenuItem(element.Key.Id);
                MenuItem item = element.Key;
                item.SetIngredient(ingredient);
                _ingredientService.SubstractStock(item);
            }
            
            return _checkoutService.FinalizeOrder(tableId, cart, restaurant);
        }

        public bool CheckTimeBetweenOrders(int? orderId)
        {
            DateTime orderTime = _cartRepository.GetOrderPlacingTime(orderId);
            DateTime now = DateTime.Now;
            TimeSpan difference = now - orderTime;
            if(difference.TotalSeconds <= 60)
            {
                return false;
            }
            return true;
        }

        public DateTime GetEndOfTimer(int orderId)
        {
            DateTime orderTime = _cartRepository.GetOrderPlacingTime(orderId);
            return orderTime + TimeSpan.FromMinutes(1);
        }

    }
}
