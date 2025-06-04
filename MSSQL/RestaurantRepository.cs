using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class RestaurantRepository: Repository
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration) { }

        public void CustomizeRestaurant(Restaurant restaurant)
        {
            string sql = @"
        UPDATE Restaurant
        SET 
            Logo = @Logo,
            Font = @Font,
            ColorButtons = @ColorButtons,
            ColorDefault = @ColorDefault,
            ColorBackground = @ColorBackground,
            IncludePayment = @IncludePayment,
            IncludeAntiAbuse = @IncludeAntiAbuse
        WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Logo", restaurant.Logo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Font", restaurant.Font ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorButtons", restaurant.ColorButtons ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorDefault", restaurant.ColorDefault ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorBackground", restaurant.ColorBackground ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IncludePayment", restaurant.IncludePayment);
                cmd.Parameters.AddWithValue("@IncludeAntiAbuse", restaurant.IncludeAntiAbuse);
                cmd.Parameters.AddWithValue("@Id", restaurant.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

       public Restaurant GetRestaurantById(int restaurantId)
        {
            string sql = @"
        SELECT Id, [Name], Email, Phone, [Address], Description, 
               Logo, Font, ColorButtons, ColorDefault, ColorBackground, KVK, IsActive, IncludePayment, IncludeAntiAbuse
        FROM Restaurant
        WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", restaurantId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Restaurant(
                            id: (int)reader["Id"],
                            name: reader["Name"].ToString(),
                            email: reader["Email"].ToString(),
                            phoneNumber: reader["Phone"].ToString(),
                            address: reader["Address"].ToString(),
                            description: reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                            logo: reader["Logo"] != DBNull.Value ? (byte[])reader["Logo"] : null,
                            font: reader["Font"] != DBNull.Value ? reader["Font"].ToString() : null,
                            colorButtons: reader["ColorButtons"] != DBNull.Value ? reader["ColorButtons"].ToString() : null,
                            colorDefault: reader["ColorDefault"] != DBNull.Value ? reader["ColorDefault"].ToString() : null,
                            colorBackground: reader["ColorBackground"] != DBNull.Value ? reader["ColorBackground"].ToString() : null,
                            isActive: reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] : false,
                            kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString() : null,
                            includePayment: reader["IncludePayment"] != DBNull.Value && (bool)reader["IncludePayment"],
                           includeAntiAbuse: reader["IncludeAntiAbuse"] != DBNull.Value && (bool)reader["IncludeAntiAbuse"]
                        );
                    }
                }
            }
            return null;
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Restaurant WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Restaurant(
                            id: (int)reader["Id"],
                            name: reader["Name"].ToString(),
                            email: reader["Email"].ToString(),
                            phoneNumber: reader["Phone"].ToString(),
                            address: reader["Address"].ToString(),
                            description: reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                            logo: reader["Logo"] != DBNull.Value ? (byte[])reader["Logo"] : null,
                            font: reader["Font"] != DBNull.Value ? reader["Font"].ToString() : null,
                            colorButtons: reader["ColorButtons"] != DBNull.Value ? reader["ColorButtons"].ToString() : null,
                            colorDefault: reader["ColorDefault"] != DBNull.Value ? reader["ColorDefault"].ToString() : null,
                            colorBackground: reader["ColorBackground"] != DBNull.Value ? reader["ColorBackground"].ToString() : null,
                            isActive: reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] : false,
                            kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString() : null,
                            includePayment: reader["IncludePayment"] != DBNull.Value && (bool)reader["IncludePayment"],
                           includeAntiAbuse: reader["IncludeAntiAbuse"] != DBNull.Value && (bool)reader["IncludeAntiAbuse"]
                );
            }

            return null;
        }


        public Restaurant GetOwnerRestaurant(int? ownerId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = @"Select r.* 
                                from Restaurant as r
                                inner join Owner as s
                                on s.RestaurantId = r.Id
                                where r.isActive = @IsActive and s.Id = @OwnerId;";

                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IsActive", 1);
                command.Parameters.AddWithValue("@OwnerId", ownerId);

                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Restaurant(
                           id: (int)reader["Id"],
                           name: reader["Name"].ToString(),
                           email: reader["Email"].ToString(),
                           phoneNumber: reader["Phone"].ToString(),
                           address: reader["Address"].ToString(),
                           description: reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                           logo: reader["Logo"] != DBNull.Value ? (byte[])reader["Logo"] : null,
                           font: reader["Font"] != DBNull.Value ? reader["Font"].ToString() : null,
                           colorButtons: reader["ColorButtons"] != DBNull.Value ? reader["ColorButtons"].ToString() : null,
                           colorDefault: reader["ColorDefault"] != DBNull.Value ? reader["ColorDefault"].ToString() : null,
                           colorBackground: reader["ColorBackground"] != DBNull.Value ? reader["ColorBackground"].ToString() : null,
                           kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString() : null,
                           includePayment: reader["IncludePayment"] != DBNull.Value && (bool)reader["IncludePayment"],
                           includeAntiAbuse: reader["IncludeAntiAbuse"] != DBNull.Value && (bool)reader["IncludeAntiAbuse"]
                       );
                }
                return null;

            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred assigning restaurant to owner: {sqlEx.Message}", sqlEx);
            }
        }
    }
}
