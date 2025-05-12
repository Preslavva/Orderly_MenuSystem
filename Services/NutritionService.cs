using Models.Entities;
using Models.Enums;
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

        public void AddNutritionToMenuItem(int menuItemId, NutritionName name, int value)
        {
            int nutritionId = _nutritionRepository.AddNutrition(name.ToString(), value);
            _nutritionRepository.AssignNutritionToMenuItem(menuItemId, nutritionId);
        }
        public void UpdateNutritions(int menuItemId, Dictionary<NutritionName, double> values)
        {
            _nutritionRepository.DeleteAllForMenuItem(menuItemId);

            foreach (var kvp in values)
            {
                int nutritionId = _nutritionRepository.AddNutrition(kvp.Key.ToString(), (int)kvp.Value);
                _nutritionRepository.AssignNutritionToMenuItem(menuItemId, nutritionId);
            }
        }


    }
}
