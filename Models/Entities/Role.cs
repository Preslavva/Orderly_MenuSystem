public class Role
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int RestaurantId { get; set; }

    public Role() { }

    public Role(int id, string type, int restaurantId)
    {
        Id = id;
        Type = type;
        RestaurantId = restaurantId;
    }

    public Role(string type, int restaurantId)
    {
        Type = type;
        RestaurantId = restaurantId;
    }
}