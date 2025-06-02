namespace MainOrderly.WebApp.ViewModels
{
    public class StaffListViewModel
    {
        public List<StaffViewModel> Staff { get; set; } = new List<StaffViewModel>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedRole { get; set; } = string.Empty;
        public bool ShowInactiveOnly { get; set; } = false;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int PreviousPage => HasPreviousPage ? PageNumber - 1 : 1;
        public int NextPage => HasNextPage ? PageNumber + 1 : TotalPages;
    }
}