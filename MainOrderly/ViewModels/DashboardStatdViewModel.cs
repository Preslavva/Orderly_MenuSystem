namespace MainOrderly.WebApp.ViewModels
{
    public class DashboardStatsViewModel
    {
        public int TotalMenuItems { get; set; }
        public int LowStockIngredients { get; set; }
        public int ActiveStaff { get; set; }
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }
    }

}