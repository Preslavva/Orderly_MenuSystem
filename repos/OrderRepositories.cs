
using Microsoft.Data.SqlClient;
using OrderlyTest.Models;
using System.Configuration;


namespace OrderlyTest.repos
{
    public class OrderRepositories
    {
        private readonly string _connectionString;

        public OrderRepositories(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Order> GetOrdersByStatus(OrderStatus status)
        {
            List<Order> orders = new();
            string query = "SELECT Id, TableId, OrderTimestamp, Status FROM [Order] WHERE Status = @Status";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Status", status.ToString());
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
         
                        int tableId = Convert.ToInt32(reader["TableId"]);
                        Table table = new Table(tableId.ToString());

                        int id = Convert.ToInt32(reader["Id"]);
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimestamp"]);


                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"]));
                        orders.Add(new Order(id, table, orderTimestamp, orderStatus));
                    }
                }
            }
            return orders;
        }


        public Order GetOrderById(int id)
        {
            Order order = null;
            string query = "SELECT Id, TableId, OrderTimestamp, Status FROM [Order] WHERE Id = @Id";

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
                        Table table = new Table(tableId.ToString());

                        int orderId = Convert.ToInt32(reader["Id"]);
                        DateTime orderTimestamp = Convert.ToDateTime(reader["OrderTimestamp"]);


                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"]));

                        order = new Order(orderId, table, orderTimestamp, orderStatus);
                    }
                }
            }
            return order;
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




    }
}
