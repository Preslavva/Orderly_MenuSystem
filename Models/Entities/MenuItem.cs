using Models.Enums;
using System.ComponentModel;
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

        public Category Category { get; set; }
        public List<Nutrition> Nutritions { get; set; }

        public MenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category)
        {
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Picture = picture;
            Category = category;
            Nutritions = new List<Nutrition>();
        }

        public MenuItem(int id, string name, string description, decimal price, bool isAvailable, string picture, Category category)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Picture = picture;
            Category = category;
            Nutritions = new List<Nutrition>();

        }

        public MenuItem() { }
    }
}
