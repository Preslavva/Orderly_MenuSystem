namespace Models.Entities;

public class RevenueEntry
{
    public int RestaurantId { get; set; }
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
}