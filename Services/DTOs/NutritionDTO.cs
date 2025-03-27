using Models.Entities;
using Models.Enums;
using System;

namespace Services.DTOs
{
    public class NutritionDTO
    {
      
        public int Id { get; set; }
        public NutritionName Name { get; set; }
        public decimal Value { get; set; }

      
        public NutritionDTO() { }

       
        public NutritionDTO(Nutrition nutrition)
        {
            Id = nutrition.Id;
            Name = nutrition.Name;
            Value = nutrition.Value;
        }

    
        public static NutritionDTO ConvertToDTO(Nutrition nutrition)
        {
            return new NutritionDTO(nutrition);
        }

    }
}