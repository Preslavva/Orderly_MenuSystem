namespace Models.Entities
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }

        public OrderItem(int orderId, int menuItemId, MenuItem menuItem, int quantity)
        {
            OrderId = orderId;
            MenuItemId = menuItemId;
            MenuItem = menuItem;
            Quantity = quantity;
        }
    }
}