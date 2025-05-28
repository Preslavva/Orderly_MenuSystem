using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;


namespace MSSQL
{
    public class KitchenOrderRepository : Repository
    {
        public KitchenOrderRepository(IConfiguration configuration) : base(configuration) { }

        public List<Order> GetOrderHeadersByStatus(OrderStatus status)
        {
            List<Order> orders = new();
            string query = "SELECT o.Id,o.TableId ,t.TableNumber , o.OrderTimestamp, o.[Status], o.RestaurantId FROM [Order] as o INNER JOIN [Table] as t on o.TableId = t.Id WHERE  o.IsArchived = 0 AND o.RestaurantId = @RestaurantId AND o.Status = @Status";

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
                        int tableNumber = Convert.ToInt32(reader["TableNumber"]);
                        Table table = new Table(tableId, new byte[0],tableNumber);
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
            string query = "SELECT o.Id, o.TableId, t.TableNumber , o.OrderTimestamp, o.[Status], o.RestaurantId FROM [Order] as o INNER JOIN [Table] as t on o.TableId = t.Id WHERE o.Id = @Id AND o.IsArchived = 0";

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
                        int tableNumber = Convert.ToInt32(reader["TableNumber"]);
                        Table table = new Table(tableId, new byte[0], tableNumber);
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
            SELECT m.Id, m.Name, m.Description, m.Price, m.IsAvailable, m.Picture, m.Category, m.RestaurantId, om.Quantity, om.OrderStatus,m.PrepTime
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
                        int prepTime = reader["PrepTime"] != DBNull.Value ? Convert.ToInt32(reader["PrepTime"]) : 0;
                        
                        var status = Enum.Parse<OrderStatus>(Convert.ToString(reader["OrderStatus"])!);

                        MenuItem menuItem = new MenuItem(menuItemId, name, description, price, isAvailable, picture, category,restaurantId,prepTime);
                        OrderItem orderItem = new OrderItem(orderId, menuItemId, menuItem, quantity,status);
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
            string sql = @"UPDATE [Order_MenuItem] SET isArchived=@isArchived WHERE OrderId = @OrderId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                foreach (var order in orderId)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@OrderId", order);
                    cmd.Parameters.AddWithValue("@isArchived", 1);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        //testing
        // 

        public List<OrderItem> GetOrderItemsByStatus(OrderStatus status)
        {
            const string sql = @"
        SELECT  om.OrderId,
                m.Id, m.Name, m.Description, m.Price, m.IsAvailable,
                m.Picture, m.Category, m.RestaurantId,
                om.Quantity,
                om.OrderStatus,
                m.PrepTime,
                o.OrderTimestamp,
                t.TableNumber
        FROM    Order_MenuItem om
        JOIN    MenuItem m ON m.Id = om.MenuItemId
        JOIN    [Order] o ON o.Id = om.OrderId
        JOIN    [Table] t ON t.Id = o.TableId
        WHERE   om.OrderStatus = @Status AND om.IsArchived = 0";

            var items = new List<OrderItem>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Status", status.ToString());
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(0);
                        int menuItemId = reader.GetInt32(1);
                        string name = reader.GetString(2);
                        string description = reader.GetString(3);
                        decimal price = reader.GetDecimal(4);
                        bool isAvailable = reader.GetBoolean(5);
                        string picture = reader.GetString(6);
                        string categoryStr = reader.GetString(7);
                        int restaurantId = reader.GetInt32(8);
                        int quantity = reader.GetInt32(9);
                        var itemStatus = Enum.Parse<OrderStatus>(reader.GetString(10));
                        int prepTime = reader["PrepTime"] != DBNull.Value
                                               ? Convert.ToInt32(reader["PrepTime"])
                                               : 0;
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimestamp"]);
                        int tableNumber = reader.GetInt32(13);
                        var category = Enum.Parse<Category>(categoryStr);
                        var menuItem = new MenuItem(menuItemId, name, description, price,
                                                    isAvailable, picture, category,
                                                    restaurantId, prepTime);

                        items.Add(new OrderItem(orderId, menuItemId, menuItem, quantity, itemStatus,orderTimestamp,tableNumber));
                    }
                }
            }
            return items;
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT om.[OrderId]
              ,om.[MenuItemId]
              ,om.[Quantity]
              ,om.[OrderStatus]
              ,om.[IsArchived]
	          ,menu.PrepTime
               FROM [dbi547761].[dbo].[Order_MenuItem] as om
               INNER JOIN MenuItem as menu on menu.Id = om.MenuItemId
                WHERE om.OrderId = @OrderId";

                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<OrderItem> orderItems = new List<OrderItem>();
                        while (reader.Read())
                        {
                            int menuItemId = reader.GetInt32(1);
                            int quantity = reader.GetInt32(2);
                            var status = Enum.Parse<OrderStatus>(reader.GetString(3));
                            int prepTime = reader["PrepTime"] != DBNull.Value
                                               ? Convert.ToInt32(reader["PrepTime"])
                                               : 0;
                            var orderItem = new OrderItem(orderId, menuItemId,new MenuItem(prepTime), quantity, status);
                            orderItems.Add(orderItem);
                        }
                        return orderItems;
                    }
                }


            }
        }

        /* ──────────────────────────────────────────────────────────────
         * 2. Update a single line-item’s status
         * ──────────────────────────────────────────────────────────────*/
        public void UpdateOrderItemStatus(int orderId, int menuItemId, OrderStatus newStatus)
        {
            const string sql = @"
        UPDATE Order_MenuItem
        SET    OrderStatus = @Status
        WHERE  OrderId     = @OrderId
          AND  MenuItemId  = @MenuItemId";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Status", newStatus.ToString());
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }


    }
}
