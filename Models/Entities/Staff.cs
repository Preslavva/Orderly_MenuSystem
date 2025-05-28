public class Staff
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public int RestaurantId { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public List<Role> Roles { get; set; } = new List<Role>();

    public Staff() { }

    public Staff(int id, string firstName, string lastName, string email, string phone, bool isActive, int restaurantId, string password, string salt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        IsActive = isActive;
        RestaurantId = restaurantId;
        Password = password;
        Salt = salt;
    }

    public Staff(string firstName, string lastName, string email, string phone, bool isActive, int restaurantId, string password, string salt)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        IsActive = isActive;
        RestaurantId = restaurantId;
        Password = password;
        Salt = salt;
    }

    public void SetRoles(List<Role> roles)
    {
        Roles = roles ?? new List<Role>();
    }

    public string FullName => $"{FirstName} {LastName}";
}