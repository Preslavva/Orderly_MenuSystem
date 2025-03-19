using Models.Enums;

namespace Models.Entities
{
    public class Nutrition
    {
        public int Id { get; set; }
        public NutritionName Name { get; set; }
        public decimal Value { get; set; }

        public Nutrition(int id, NutritionName name, decimal value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public Nutrition(NutritionName name, decimal value)
        {
            Name = name;
            Value = value;
        }

        public Nutrition() { }
    }
}
