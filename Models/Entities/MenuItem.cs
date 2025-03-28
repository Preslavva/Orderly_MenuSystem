using Models.Enums;
using System.Net.Http.Headers;


namespace Models.Entities
{
    public class MenuItem
    {
        public int Id { get;  }

        public string Name { get;  }

        public string Description { get;  }

        public decimal Price { get; }

        public bool IsAvailable { get; }

        public string Picture { get; }

        public int Quantity { get; }

        public Category Category { get; set; }
        public List<Nutrition> Nutritions { get;}

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

        public MenuItem(string name, string description, decimal price, string picture, int quantity, Category category)
        {
            Name = name;
            Description = description;
            Price = price;
            Picture = picture;
            Quantity = quantity;
            Category = category;
        }
     
        public MenuItem() { }
    }
}
