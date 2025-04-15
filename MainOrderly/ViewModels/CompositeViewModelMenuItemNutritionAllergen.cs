namespace MainOrderly.WebApp.ViewModels
{
    public class CompositeViewModelMenuItemNutritionAllergen
    {
        public MenuItemViewModel MenuItemViewModel { get; set; }
        public List<NutritionViewModel> NutritionViewModel { get; set; }

        public List<AllergenViewModel> AllergenViewModel { get; set; }

        public List<IngredientViewModel> IngredientViewModel { get; set; }  

    }
}
