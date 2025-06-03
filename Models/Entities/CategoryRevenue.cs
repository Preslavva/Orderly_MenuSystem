namespace Models.Entities;

public class CategoryRevenue(string categoryName, decimal totalRevenue)
{
    public int RestaurantId { get; set; }
    public string CategoryName { get; set; } = categoryName;
    public decimal TotalRevenue { get; set; } = totalRevenue;
}