using Models.Entities;

namespace MainOrderly.WebApp.ViewModels
{
    public class IngredientViewModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Unit { get; set; }
        public int QuantityInStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public string FormattedStock => $"{QuantityInStock} {Unit}";


        public static IngredientViewModel ConvertToViewModel(Ingredient ingredient)
        {
            return new IngredientViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                QuantityInStock = ingredient.QuantityInStock,
                MinimumStockLevel = ingredient.MinimumStockLevel,

            };
        }

    }


   
}
