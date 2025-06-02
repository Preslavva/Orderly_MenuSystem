namespace MainOrderly.WebApp.ViewModels
{
    public class OwnerDashboardViewModel
    {
        public OwnerViewModel Owner { get; set; }
        public RestaurantViewModel Restaurant { get; set; }
        public DashboardStatsViewModel Stats { get; set; }
    }
}