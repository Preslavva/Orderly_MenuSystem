using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class MonthlyRevenue
    {
        public int RestaurantId { get; set; }
        public string Month { get; set; }
        public string year { get; set; }
        public decimal Revenue { get; set; }
        public MonthlyRevenue(int restaurantId, string month, decimal revenue)
        {
            RestaurantId = restaurantId;
            Month = month;
            Revenue = revenue;
        }
      
    }
}
