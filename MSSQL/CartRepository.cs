using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class CartRepository : Repository
    {
        public CartRepository(IConfiguration configuration) : base(configuration) { }

        public void AddMenuItemToOrder(int orderId, int menuItemId, int quantity)
        {
            string sql = @"
                INSERT INTO [Order_MenuItem] (OrderId, MenuItemId, Quantity)
                VALUES (@OrderId, @MenuItemId, @Quantity);";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
