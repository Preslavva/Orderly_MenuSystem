using System;
using System.Collections.Generic;

namespace Models.Entities
{
    public class Owner
    {
        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Password { get; }
        public string Salt { get; }
        public int RestaurantId { get; }

        public Owner(int id, string firstName, string lastName, string email, string phone, string password,
            string salt, int restaurantId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Password = password;
            Salt = salt;
            RestaurantId = restaurantId;
        }

        public Owner(string firstName, string lastName, string email, string phone, string password, string salt,
            int restaurantId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Password = password;
            Salt = salt;
            RestaurantId = restaurantId;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}