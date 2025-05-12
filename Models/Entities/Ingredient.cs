using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Ingredient
    {
        public int Id { get; }

        public int RestaurantId { get; }
        public string Name { get;}
        public string Unit { get;}
        public int  QuantityInStock { get; private set; }
        public int MinimumStockLevel { get; }

        public Ingredient(int id, string name, string unit, int quantityInStock, int minimumStockLevel, int restaurantId)
        {
            Id = id;
            Name = name;
            Unit = unit;
            QuantityInStock = quantityInStock;
            MinimumStockLevel = minimumStockLevel;
            RestaurantId =  restaurantId;
        }

        public void SetQuantityInStock(int quantity)
        {
            QuantityInStock = quantity;
        }


    }
}
