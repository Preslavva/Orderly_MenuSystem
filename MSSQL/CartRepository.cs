using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;

namespace MSSQL
{
    public class CartRepository : Repository
    {
        public CartRepository(IConfiguration configuration) : base(configuration) { }

        public void AddMenuItemToOrder(int orderId, int menuItemId, int quantity, OrderStatus status, int restaurantId)
        {
            string sql = @"
                INSERT INTO [Order_MenuItem] (OrderId, MenuItemId, Quantity,OrderStatus, IsArchived)
                SELECT @OrderId, @MenuItemId, @Quantity, @OrderStatus, @IsArchived
                FROM [Order] o
                INNER JOIN MenuItem m ON m.Id = @MenuItemId
                WHERE o.Id = @OrderId AND o.RestaurantId = @RestaurantId AND m.RestaurantId = @RestaurantId;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@OrderStatus", status.ToString());
                cmd.Parameters.AddWithValue("@IsArchived", 0);
                cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DateTime GetOrderPlacingTime(int? orderId, int restaurantId)
        {
            DateTime orderTimestamp = DateTime.MinValue;
            using SqlConnection connection = new SqlConnection(_connectionString);  
            connection.Open();

            string sql = @"select OrderTimeStamp
                           from [Order]
                           where Id = @Id AND RestaurantId = @RestaurantId";
            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", orderId);
            command.Parameters.AddWithValue("@RestaurantId", restaurantId);

            using SqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                orderTimestamp = reader.GetDateTime(0);
            }
            return orderTimestamp;
        }
    }
}
