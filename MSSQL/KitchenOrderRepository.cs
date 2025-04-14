using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;
using System.Configuration;


namespace MSSQL
{
    public class KitchenOrderRepository : Repository
    {
        public KitchenOrderRepository(IConfiguration configuration) : base(configuration) { }

        public List<Order> GetOrderHeadersByStatus(OrderStatus status)
        {
            List<Order> orders = new();
            string query = "SELECT Id, TableId, OrderTimestamp, [Status], RestaurantId FROM [Order] WHERE [Status] = @Status AND RestaurantId = @RestaurantId AND isArchived = 0";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
               
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@RestaurantId", 1);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        int tableId = Convert.ToInt32(reader["TableId"]);
                        Table table = new Table(tableId, new byte[0]);
                        int id = Convert.ToInt32(reader["Id"]);
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimestamp"]);
                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"])!);
                        int restaurantId = Convert.ToInt32(reader["RestaurantId"]);

                        Order order = new Order(id, table, orderTimestamp, orderStatus, restaurantId );
                        orders.Add(order);

                    }

                }
            }
            return orders;
        }

        public Order GetOrderHeaderById(int id)
        {
            Order order = null!;
            string query = "SELECT Id, TableId, OrderTimestamp, [Status], RestaurantId FROM [Order] WHERE Id = @Id AND isArchived = 0";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int tableId = Convert.ToInt32(reader["TableId"]);
                        Table table = new Table(tableId, new byte[0]);
                        int orderId = Convert.ToInt32(reader["Id"]);
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimestamp"]);
                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"])!);
                        int restaurantId = Convert.ToInt32(reader["RestaurantId"]);

                        order = new Order(orderId, table, orderTimestamp, orderStatus, restaurantId);
                    }
                }
            }
            return order!;
        }
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            string query = @"
            SELECT m.Id, m.Name, m.Description, m.Price, m.IsAvailable, m.Picture, m.Category, m.RestaurantId, om.Quantity, m.PrepTime
            FROM [Order_MenuItem] om
            INNER JOIN MenuItem m ON om.MenuItemId = m.Id
            WHERE om.OrderId = @OrderId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int menuItemId = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string description = reader.GetString(2);
                        decimal price = reader.GetDecimal(3);
                        bool isAvailable = reader.GetBoolean(4);
                        string picture = reader.GetString(5);
                        string categoryString = reader.GetString(6);
                        int quantity = reader.GetInt32(8);
                        Category category = (Category)Enum.Parse(typeof(Category), categoryString);
                        int restaurantId = Convert.ToInt32(reader.GetInt32(7));
                        int prepTime = reader["PrepTime"] != DBNull.Value
    ? Convert.ToInt32(reader["PrepTime"])
    : 0;

                        MenuItem menuItem = new MenuItem(menuItemId, name, description, price, isAvailable, picture, category,restaurantId,prepTime);
                        OrderItem orderItem = new OrderItem(orderId, menuItemId, menuItem, quantity);
                        orderItems.Add(orderItem);
                    }
                }
            }
            return orderItems;
        }


        public void UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            string query = "UPDATE [Order] SET Status = @Status WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Status", newStatus.ToString());
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int CreateOrder(int tableId, OrderStatus status, int totalQuantity, decimal totalPrice, int restaurantId)
        {
            string sql = @"
            INSERT INTO [Order] (TableId, OrderTimestamp, Status, Quantity, SubTotal, RestaurantId)
            OUTPUT INSERTED.Id
            VALUES (@TableId, @Timestamp, @Status, @Quantity, @SubTotal, @RestaurantId);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableId", tableId);
                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", status.ToString());
                cmd.Parameters.AddWithValue("@Quantity", totalQuantity);
                cmd.Parameters.AddWithValue("@SubTotal", totalPrice);
                cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                conn.Open();
                int newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                return newOrderId;
            }
        }

        public void RemoveOrder(List<int> orderId)
        {
            string sql = @"UPDATE [Order] SET isArchived=@isArchived WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                foreach (var order in orderId)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Id", order);
                    cmd.Parameters.AddWithValue("@isArchived", 1);
                    cmd.ExecuteNonQuery();
                }

              
               
            }
        }

    
    }
}
