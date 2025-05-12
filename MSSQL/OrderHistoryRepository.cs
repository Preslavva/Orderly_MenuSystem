using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;

namespace MSSQL
{
    public class OrderHistoryRepository: Repository
    {
        public OrderHistoryRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public List<OrderHistory> GetHistoryOrders(int orderId)
        {
            List<OrderHistory> history = new List<OrderHistory>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"select o.Id ,m.[Name], om.Quantity, m.Price, o.SubTotal, o.[Status], o.OrderTimeStamp
                                                from [Order] as o 
                                                inner join Order_MenuItem as om
                                                on om.OrderId = o.Id
                                                inner join MenuItem as m
                                                on m.Id = om.MenuItemId
                                                where o.Id = @Id ";
                                                
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", orderId);
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        history.Add(new OrderHistory(
                            Convert.ToInt32(reader["Id"]),
                            Convert.ToString(reader["Name"]),
                            Convert.ToInt32(reader["Quantity"]),
                            Convert.ToDecimal(reader["Price"]),
                            Convert.ToDecimal(reader["SubTotal"]),
                            Enum.Parse<OrderStatus>(Convert.ToString(reader["Status"])!),
                            Convert.ToDateTime(reader["OrderTimeStamp"])
                            ));
                    }
                }
            }
            return history;
        }
    }
}
