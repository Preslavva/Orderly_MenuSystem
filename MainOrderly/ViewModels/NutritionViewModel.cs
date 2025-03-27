using Models.Entities;
using Models.Enums;

namespace MainOrderly.WebApp.ViewModels
{
    public class NutritionViewModel
    {
        public int Id { get; set; }
        public NutritionName Name { get; set; }
        public decimal Value { get; set; }

        public static NutritionViewModel ConvertToViewModel(Nutrition nutrition)
        {
            return new NutritionViewModel
            {
                Id = nutrition.Id,
                Name = nutrition.Name,
                Value = nutrition.Value
            };
        }
    }
}
