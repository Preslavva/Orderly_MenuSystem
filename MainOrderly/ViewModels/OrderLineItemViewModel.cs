using Models.Entities;

public class OrderLineItemViewModel
{
    public int MenuItemId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsExceeded { get; set; }

    // This will be used when converting from OrderMenuItem
    public static OrderLineItemViewModel FromOrderMenuItem(OrderItem orderMenuItem)
    {
        return new OrderLineItemViewModel
        {
            MenuItemId = orderMenuItem.MenuItem.Id,
            Name = orderMenuItem.MenuItem.Name,
            Price = orderMenuItem.MenuItem.Price,
            Quantity = orderMenuItem.Quantity
        };
    }
}