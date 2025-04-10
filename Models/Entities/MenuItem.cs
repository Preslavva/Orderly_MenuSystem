using Models.Enums;
using System.Net.Http.Headers;


namespace Models.Entities
{
    public class MenuItem
    {
        public int Id { get;  }
        public int RestaurantId { get; set; }
        public string Name { get;  }
        public string Description { get;  }
        public decimal Price { get; }
        public bool IsAvailable { get; }
        public string Picture { get; }
        public int Quantity { get; }
        public int PrepTime { get; }
        public Category Category { get; set; }
        public List<Nutrition> Nutritions { get;}
        public List<MenuItemIngredient> Ingredients { get; private set; }

        public MenuItem(int id, string name, string description, decimal price, bool isAvailable, string picture, Category category, int restaurantId, int prepTime)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Picture = picture;
            Category = category;
            Nutritions = new List<Nutrition>();
            RestaurantId = restaurantId;
            PrepTime = prepTime;
        }

        public MenuItem(string name, string description, decimal price, string picture, int quantity, Category category, int restaurantId )
        {
            Name = name;
            Description = description;
            Price = price;
            Picture = picture;
            Quantity = quantity;
            Category = category;
            RestaurantId= restaurantId;
        }
     
        public void SetIngredient(List<MenuItemIngredient> ingredients)
        {
            Ingredients = ingredients;
        }
    }
}
