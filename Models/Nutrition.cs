namespace OrderlyTest.Models
{
    public class Nutrition
    {
        public int Id { get; }
        public NutritionName Name { get; set; }
        public decimal Value { get; set; }

        public Nutrition(int id, NutritionName name, decimal value)
        {
            this.Id = id;
            this.Name = name;   
            this.Value = value;
        }

        public Nutrition(NutritionName name, decimal value)
        {
            this.Name=name;
            this.Value = value;
        }
    }
}
