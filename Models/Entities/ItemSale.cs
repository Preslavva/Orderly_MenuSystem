namespace Models.Entities;

public class ItemSale
{
    public int RestaurantId { get; set; }
    public string MenuItemName { get; set; }
    public int QuantitySold { get; set; }
}