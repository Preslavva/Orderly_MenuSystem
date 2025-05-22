using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;

namespace MSSQL
{
    public class WaiterRepository : Repository
    {
        public WaiterRepository(IConfiguration configuration) : base(configuration) { }

        public List<Order> GetCompletedOrdersWithItems()
        {
            var orders = new List<Order>();
            string query = """
                SELECT o.Id AS OrderId, o.TableId, o.OrderTimeStamp, o.Status, o.RestaurantId,
                oi.MenuItemId, oi.Quantity, 
                mi.Name, mi.Price, mi.PrepTime,
                t.TableNumber
                FROM [ORDER] o
                JOIN Order_MenuItem oi ON o.Id = oi.OrderId
                JOIN MenuItem mi ON oi.MenuItemId = mi.Id
                JOIN [Table] t ON o.TableId = t.Id
                WHERE o.Status = @Status
                ORDER BY o.Id;
                """;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Status", OrderStatus.COMPLETED.ToString());
                command.Parameters.AddWithValue("@RestaurantId", 1);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Dictionary<int, Order> ordermap = new();

                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["OrderId"]);
                        int tableId = Convert.ToInt32(reader["TableId"]);
                        int tableNumber = Convert.ToInt32(reader["TableNumber"]);
                        Table table = new Table(tableId, tableNumber);
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimeStamp"]);
                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"])!);
                        int restaurantId = Convert.ToInt32(reader["RestaurantId"]);

                        if (!ordermap.ContainsKey(orderId))
                        {
                            ordermap[orderId] = new Order(orderId, table, orderTimestamp, orderStatus, restaurantId);
                        }

                        var menuItem = new MenuItem((int)reader["MenuItemId"], reader["Name"].ToString(), (decimal)reader["Price"], Convert.ToInt32(reader["PrepTime"]));
                        var item = new OrderItem(orderId, menuItem, (int)reader["Quantity"], (DateTime)reader["OrderTimestamp"]);

                        ordermap[orderId].Items.Add(item);

                    }

                    orders = ordermap.Values.ToList();

                }
            }

            return orders;
        }

        public void UpdateOrderStatusDelivered(int id)
        {
            string query = "UPDATE [Order] SET Status = @Status WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Status", OrderStatus.DELIVERED.ToString());
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
