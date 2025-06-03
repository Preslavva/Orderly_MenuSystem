using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class AnalyticsService
    {
        private readonly AnalyticsRepository _analyticsRepository;
        
        public AnalyticsService(AnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public List<RevenueEntry> GetRevenueByDataRange(int year, int month,int restaurantId) =>
            _analyticsRepository.GetRevenue(restaurantId,year,month);

        public List<HourlyOrder> GetHourlyOrders(int restaurantId,int month, int year) =>
            _analyticsRepository.GetHourlyOrders(restaurantId,year,month);

        public List<ItemSale> GetTopSellingItems(int year, int month, int restaurantId) =>
            _analyticsRepository.GetBestSellingItems(year, month, restaurantId);

        public List<CategoryRevenue> GetCategoryRevenues(int year, int month, int restaurantId) =>
            _analyticsRepository.GetCategoryRevenues(restaurantId, year, month);
    }
}
