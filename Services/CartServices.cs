using Microsoft.AspNetCore.Http;
using Models;
using Models.Entities;
using MSSQL;
using System.Collections.Generic;
using System.Text.Json;
namespace Services
{
    public class CartServices
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly MenuItemRepository _menuItemRepository;
        private Dictionary<int, int> cart;
        private readonly CartRepository _cartRepository;
        private readonly CheckoutService _checkoutService;

        public CartServices(IHttpContextAccessor contxtAccessor, MenuItemRepository menuDB, CartRepository cartRepository, CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
            _cartRepository = cartRepository;
            contextAccessor = contxtAccessor;
            _menuItemRepository = menuDB;
            cart = new Dictionary<int, int>();

            string? jsonCart = contextAccessor.HttpContext?.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(jsonCart))
            {
                cart = JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCart)!;
            }

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

        public void UpdateQuantity(int id, int newQuanity)
        {
            if (cart.ContainsKey(id))
            {
                cart[id] = newQuanity;
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
            contextAccessor.HttpContext?.Session.SetString("Cart", jsonCart);
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

        public Dictionary<MenuItem, int> GetCart()
        {
            Dictionary<MenuItem, int> newCart = new Dictionary<MenuItem, int>();
            string? jsonCart = contextAccessor.HttpContext?.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(jsonCart))
            {
                cart = JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCart)!;
                foreach (int key in cart.Keys)
                {
                    MenuItem? item = _menuItemRepository.GetMenuItemById(key);
                    newCart[item!] = cart[key];
                }
            }
            return newCart;
        }

        public decimal CalculateTotalPrice(Dictionary<MenuItem, int> cart)
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

        public int FinalizeOrder(int tableId)
        {
            Dictionary<MenuItem, int> cart = GetCart();

            return _checkoutService.FinalizeOrder(tableId, cart);
        }
    }
}
