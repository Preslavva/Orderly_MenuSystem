using Models.Enums;

namespace Models.Entities
{
    public class OrderItem
    { 
        public int TableNumber { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderTimestamp { get; set; }


        public OrderItem(int orderId, int menuItemId, MenuItem menuItem, int quantity,OrderStatus status,DateTime orderTimestamp,int tableNumber)
        {
            OrderId = orderId;
            MenuItemId = menuItemId;
            MenuItem = menuItem;
            Quantity = quantity;
            Status = status;
            OrderTimestamp = orderTimestamp;
            TableNumber = tableNumber;
        }

        public OrderItem(int orderId, int menuItemId, MenuItem menuItem, int quantity, OrderStatus status)
        {
            OrderId = orderId;
            MenuItemId = menuItemId;
            MenuItem = menuItem;
            Quantity = quantity;
            Status = status;
      
        }




    }
}