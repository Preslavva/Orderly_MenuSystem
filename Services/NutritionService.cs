using Models.Entities;
using MSSQL;

namespace Services
{
    public class NutritionService
    {
        private readonly NutritionRepository _nutritionRepository;

        public NutritionService(NutritionRepository nutritionRepository)
        {
            _nutritionRepository = nutritionRepository;
        }


        public List<Nutrition> GetNutritionForMenuItem(int id)
        {
            return _nutritionRepository.GetNutritionsForMenuItem(id)!;
        }
    }
}
