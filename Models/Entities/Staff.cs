public class Staff
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string Phone { get; }
    public bool IsActive { get; }
    public int RestaurantId { get; }
    public string Password { get; }
    public string Salt { get; }
    public List<Role> Roles { get; private set; } = new List<Role>();

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
        Roles = roles;
    }

    public string FullName => $"{FirstName} {LastName}";
}