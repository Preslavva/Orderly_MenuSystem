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

        public MenuItem(string name, string description, decimal price, bool isAvailable)
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
        }

        public MenuItem(int id,string name, string description, decimal price, bool isAvailable)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.IsAvailable = isAvailable;
        }
    }
}
