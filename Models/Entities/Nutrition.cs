using Models.Enums;

namespace Models.Entities
{
    public class Nutrition
    {
        public int Id { get; set; }
        public NutritionName Name { get; set; }
        public int Value { get; set; }

        public Nutrition(int id, NutritionName name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public Nutrition(NutritionName name, int value)
        {
            Name = name;
            Value = value;
        }

        public Nutrition() { }
    }
   
}
