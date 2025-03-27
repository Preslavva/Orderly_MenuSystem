using Models.Enums;
using System.Net.Http.Headers;

namespace Models.Entities
{
    public class MenuItem
    {
        public int Id { get; }

        public string Name { get;  }

        public string Description { get; }

        public decimal Price { get; }

        public bool IsAvailable { get; }

        public string Picture { get; }

        public int Quantity { get; }

        public Continent Continent { get; }
        public List<Nutrition> Nutritions { get; }

        public MenuItem(string name, string description, decimal price, bool isAvailable, string picture, int quantity, Continent continent)
        {
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Picture = picture;
            Quantity = quantity;
            Continent = continent;
            Nutritions = new List<Nutrition>();
        }

        public MenuItem(int id, string name, string description, decimal price, bool isAvailable, string picture, int quantity, Continent continent)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Picture = picture;
            Quantity = quantity;
            Continent = continent;
            Nutritions = new List<Nutrition>();

        }

        public MenuItem() { }
    }
}
