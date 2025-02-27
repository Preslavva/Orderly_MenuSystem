using System.Net.Http.Headers;

namespace OrderlyTest.Models
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

        public List<Nutrition> Nutritions { get; set; }

        public MenuItem(string name, string description, decimal price, bool isAvailable, string picture, int quantity)
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
            this.Picture = picture;
            this.Quantity = quantity;
            this.Nutritions = new List<Nutrition>();
        }

        public MenuItem(int id,string name, string description, decimal price, bool isAvailable, string picture, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
            this.Picture = picture;
            this.Quantity = quantity;
            this.Nutritions = new List<Nutrition>();

        }

        public MenuItem() { }
    }
}
