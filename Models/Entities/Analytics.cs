using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Analytics
    {
        public List<RevenueEntry> RevenueEntries { get; set; } = new List<RevenueEntry>();
        public List<HourlyOrder> HourlyOrders { get; set; } = new List<HourlyOrder>();
        public List<ItemSale> ItemSales { get; set; } = new List<ItemSale>();
    }
}

    
