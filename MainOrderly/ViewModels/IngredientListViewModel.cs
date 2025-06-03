namespace MainOrderly.WebApp.ViewModels
{
    public class IngredientListViewModel
    {
        public List<IngredientViewModel> Ingredients { get; set; } = new();
        public PaginationModel Pagination { get; set; } = new PaginationModel();
    }
}
