using System.Net.Http.Headers;

namespace OrderlyTest.Models
{
    public class MenuItem
    {
        public int Id { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        bool IsAvailable { get; set; }

        public string Picture { get; set; }

        public MenuItem(string name, string description, decimal price, bool isAvailable, string picture)
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
            this.Picture = picture;
        }

        public MenuItem(int id,string name, string description, decimal price, bool isAvailable, string picture)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
            this.Picture = picture;
        }
    }
}
