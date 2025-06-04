using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class AnalyticsRepository : Repository
    {
        public AnalyticsRepository(IConfiguration configuration) : base(configuration) {}
        //WHERE MONTH(o.OrderTimeStamp) = 6 AND YEAR(o.OrderTimeStamp)= 2025
        

        public List<HourlyOrder> GetHourlyOrders(int restaurantId,int year, int month)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Base SQL
            string query = @"WITH HourlyRange AS (
    SELECT 8 AS Hour 
    UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 
    UNION SELECT 12 UNION SELECT 13 UNION SELECT 14 UNION SELECT 15
    UNION SELECT 16 UNION SELECT 17 UNION SELECT 18 UNION SELECT 19
    UNION SELECT 20 UNION SELECT 21 UNION SELECT 22 UNION SELECT 23 UNION SELECT 24
),
DistinctRestaurants AS (
    SELECT DISTINCT RestaurantId 
    FROM [Order]
    WHERE RestaurantId = 21
),
AllCombos AS (
    SELECT r.RestaurantId, h.Hour
    FROM DistinctRestaurants r
    CROSS JOIN HourlyRange h
),
OrderCounts AS (
    SELECT 
        RestaurantId,
        DATEPART(HOUR, OrderTimeStamp) AS OrderHour,
        COUNT(*) AS OrderCount
    FROM [Order]
    WHERE DATEPART(HOUR, OrderTimeStamp) BETWEEN 8 AND 24 
          AND MONTH(OrderTimeStamp) = @Month
          AND YEAR(OrderTimeStamp) = @Year 
          AND RestaurantId = @RestaurantId
    GROUP BY RestaurantId, DATEPART(HOUR, OrderTimeStamp)
)
SELECT 
    ac.RestaurantId,
    ac.Hour,
    ISNULL(oc.OrderCount, 0) AS OrderCount
FROM AllCombos ac
LEFT JOIN OrderCounts oc 
    ON ac.RestaurantId = oc.RestaurantId AND ac.Hour = oc.OrderHour
ORDER BY ac.RestaurantId, ac.Hour;
";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            
            using SqlDataReader reader = cmd.ExecuteReader();
            List<HourlyOrder> hourlyOrders = new();

            while (reader.Read())
            {
                Console.WriteLine($"RestaurantId: {reader["RestaurantId"]}, Hour: {reader["Hour"]}, OrderCount: {reader["OrderCount"]}");
                hourlyOrders.Add(new HourlyOrder(
                    Convert.ToInt32(reader["RestaurantId"]),
                    Convert.ToInt32(reader["OrderCount"]),
                    Convert.ToInt32(reader["Hour"])
                ));
            }

            return hourlyOrders;
        }


        public List<ItemSale> GetBestSellingItems(int year, int month,int restaurantId)
        {

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT m.Name,o.RestaurantId,
            SUM(o.Quantity) AS TotalQuantitySold
            FROM [dbi547761].[dbo].[Order] o
            INNER JOIN Order_MenuItem om ON om.OrderId = o.Id
            INNER JOIN MenuItem m ON m.Id = om.MenuItemId
            WHERE MONTH(o.OrderTimeStamp) = @Month AND YEAR(o.OrderTimeStamp)= @Year AND o.RestaurantId = @RestaurantId
            GROUP BY m.Name,o.RestaurantId
            ORDER BY TotalQuantitySold DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
          
            using SqlDataReader reader = cmd.ExecuteReader();
            List<ItemSale> bestSellingItems = new List<ItemSale>();
            while (reader.Read())
            {
                bestSellingItems.Add(new ItemSale
                {
                    MenuItemName = reader["Name"].ToString(),
                    QuantitySold = Convert.ToInt32(reader["TotalQuantitySold"]),
                    RestaurantId = Convert.ToInt32(reader["RestaurantId"])
                });
            }
            return bestSellingItems;

        }

        public int GetTotalOrders(int restaurantId, int month, int year)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"
            SELECT COUNT(*) AS TotalOrders
            FROM [Order]
            WHERE MONTH(OrderTimeStamp) = @Month AND YEAR(OrderTimeStamp) = @Year AND RestaurantId = @RestaurantId;
            ";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public decimal GetTotalRevenue(int restaurantId, int month, int year)
        {

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"
            SELECT SUM(SubTotal) AS TotalRevenue
            FROM [Order]
            WHERE MONTH(OrderTimeStamp) = @Month AND YEAR(OrderTimeStamp) = @Year AND RestaurantId = @RestaurantId;
            ";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
        }

                
        public List<RevenueEntry> GetRevenue(int restaurantId,int? year, int? month)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string baseQuery = @"
            SELECT 
                CAST(OrderTimeStamp AS DATE) AS OrderDate,
                RestaurantId,
                SUM(SubTotal) AS TotalSubTotal
             FROM [Order]
             WHERE MONTH(OrderTimeStamp) = @Month AND YEAR(OrderTimeStamp) = @Year AND RestaurantId = @RestaurantId
               GROUP BY 
                RestaurantId, 
                CAST(OrderTimeStamp AS DATE)
            ORDER BY 
                OrderDate, RestaurantId;
             ";

            using SqlCommand cmd = new SqlCommand(baseQuery, conn);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);

            using SqlDataReader reader = cmd.ExecuteReader();
            List<RevenueEntry> revenueEntries = new();

            while (reader.Read())
            {
                revenueEntries.Add(new RevenueEntry
                {
                    Date = (DateTime)reader["OrderDate"],
                    RestaurantId = (int)reader["RestaurantId"],
                    Revenue = (decimal)reader["TotalSubTotal"]
                });
            }

            return revenueEntries;
        }

        public List<CategoryRevenue> GetCategoryRevenues(int restaurantId, int year, int month)
        {

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"
            SELECT m.Category, SUM(o.SubTotal) AS Revenue
            FROM [Order] o
			inner JOIN Order_MenuItem as om on om.OrderId = o.Id
            inner JOIN MenuItem as m ON  om.MenuItemId = m.Id
            WHERE MONTH(OrderTimeStamp) = @Month AND YEAR(OrderTimeStamp) = @Year AND o.RestaurantId = @RestaurantId
            GROUP BY Category
            ";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
            using SqlDataReader reader = cmd.ExecuteReader();
            List<CategoryRevenue> categoryRevenues = new();
            while (reader.Read())
            {
                categoryRevenues.Add(new CategoryRevenue(

                     Convert.ToString(reader["Category"])!,
                     Convert.ToDecimal(reader["Revenue"])
                    )
                );
            }
            return categoryRevenues;
        }






    }
}
