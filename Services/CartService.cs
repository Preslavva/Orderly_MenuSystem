using Microsoft.AspNetCore.Http;
using Models;
using Models.Entities;
using MSSQL;
using System.Collections.Generic;
using System.Text.Json;
namespace Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly MenuItemRepository _menuItemRepository;
        private Dictionary<int, int> cart;
        private readonly CartRepository _cartRepository;
        private readonly CheckoutService _checkoutService;

        public CartService(IHttpContextAccessor contxtAccessor, MenuItemRepository menuItemRepository, CartRepository cartRepository, CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
            _cartRepository = cartRepository;
            contextAccessor = contxtAccessor;
            _menuItemRepository = menuItemRepository;
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
            
        public Dictionary<MenuItemDTO, int> GetCart()
        {
            Dictionary<MenuItemDTO, int> newCart = new Dictionary<MenuItemDTO, int>();
            string? jsonCart = contextAccessor.HttpContext?.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(jsonCart))
            {
                cart = JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCart)!;
                foreach (int key in cart.Keys)
                {
                    MenuItem? item = _menuItemRepository.GetMenuItemById(key);
                    
                    MenuItemDTO? itemDTO = MenuItemDTO.ConvertToDTO(item);
                    newCart[itemDTO!] = cart[key];
                }
            }
            return newCart;
        }

        public decimal CalculateTotalPrice(Dictionary<MenuItemDTO, int> cart)
        {
            decimal totalPrice = 0;
            foreach (var element in cart)
            {
                MenuItemDTO item = element.Key;
                int quantity = element.Value;
                totalPrice += quantity * item.Price;
            }
            return totalPrice;
        }

        public int FinalizeOrder(int tableId)
        {
            Dictionary<MenuItemDTO, int> cart = GetCart();

            return _checkoutService.FinalizeOrder(tableId, cart);
        }
    }
}
