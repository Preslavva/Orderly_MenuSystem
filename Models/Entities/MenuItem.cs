using Models.Enums;
using System.Net.Http.Headers;

namespace Models.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public string Picture { get; set; }

        public int Quantity { get; set; }

        public Continent Continent { get; set; }
        public List<Nutrition> Nutritions { get; set; }

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
