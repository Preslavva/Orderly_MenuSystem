namespace Models.Entities;

public class HourlyOrder(int restaurantId, int orderCount, int hour)
{
    public int RestaurantId { get; set; } = restaurantId;
    public int Hour { get; set; } = hour;
    public int OrderCount { get; set; } = orderCount;
}