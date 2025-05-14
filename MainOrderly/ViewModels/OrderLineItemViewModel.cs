using System.Runtime.InteropServices.JavaScript;
using Models.Entities;
using Models.Enums;

public class OrderLineItemViewModel
{
    /* ──────── Order-level context ──────── */
    public int OrderId { get; set; }
    public int Table { get; set; }
    public string ElapsedTime { get; set; } = "00:00";
    public OrderStatus Status { get; set; }
    public bool IsExceeded { get; set; }

    /* ──────── Menu-item specifics ──────── */
    public int MenuItemId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int PrepTime { get; set; }

    public string OrderTimestamp { get; set; }

    /* ───────────────────────────────────── */
    public static OrderLineItemViewModel FromOrderItem(OrderItem orderItem,int tableNumber, string elapsedTime,
        bool isExceeded)
    {
        return new OrderLineItemViewModel
        {
            /* order-level */
            OrderId = orderItem.OrderId,
            Table = tableNumber,
            ElapsedTime = elapsedTime,
            Status = orderItem.Status,
            IsExceeded = isExceeded,

            /* menu-item */
            MenuItemId = orderItem.MenuItem.Id,
            Name = orderItem.MenuItem.Name,
            Price = orderItem.MenuItem.Price,
            Quantity = orderItem.Quantity,
            PrepTime = orderItem.MenuItem.PrepTime,
            OrderTimestamp = orderItem.OrderTimestamp.ToString("HH:mm:ss")
        };
    }
}