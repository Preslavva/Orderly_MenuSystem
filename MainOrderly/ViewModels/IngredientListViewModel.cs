namespace MainOrderly.WebApp.ViewModels
{
    public class IngredientListViewModel
    {
        public List<IngredientViewModel> Ingredients { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
    }
}
