using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class RestaurantRepository: Repository
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration) { }

        public void CreateRestaurant(Restaurant restaurant)
        {

            string sql = @"
                INSERT INTO Restaurant([Name], Email, Phone, [Address], Description, KVK, isActive )
                VALUES(@Name, @Email, @Phone, @Address, @Description, @KVK, @isActive)";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Name", restaurant.Name);
                cmd.Parameters.AddWithValue("@Email", restaurant.Email);
                cmd.Parameters.AddWithValue("@Phone", restaurant.PhoneNumber);
                cmd.Parameters.AddWithValue("@Address", restaurant.Address);
                cmd.Parameters.AddWithValue("@Description", restaurant.Description);
				cmd.Parameters.AddWithValue("@KVK", restaurant.KVK);
				cmd.Parameters.AddWithValue("@isActive", 1);


                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateRestaurant(Restaurant restaurant)
        {
            string sql = @"
        UPDATE Restaurant
        SET 
            [Name] = @Name,
            Email = @Email,
            Phone = @Phone,
            [Address] = @Address,
            Description = @Description,
            Logo = @Logo,
            Font = @Font,
            ColorButtons = @ColorButtons,
            ColorDefault = @ColorDefault,
            ColorBackground = @ColorBackground,
            KVK = @KVK
        WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Name", restaurant.Name);
                cmd.Parameters.AddWithValue("@Email", restaurant.Email);
                cmd.Parameters.AddWithValue("@Phone", restaurant.PhoneNumber);
                cmd.Parameters.AddWithValue("@Address", restaurant.Address);
                cmd.Parameters.AddWithValue("@Description", restaurant.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Logo", restaurant.Logo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Font", restaurant.Font ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorButtons", restaurant.ColorButtons ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorDefault", restaurant.ColorDefault ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColorBackground", restaurant.ColorBackground ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@KVK", restaurant.KVK ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@Id", restaurant.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Restaurant GetRestaurantById(int restaurantId)
        {
            string sql = @"
        SELECT Id, [Name], Email, Phone, [Address], Description, 
               Logo, Font, ColorButtons, ColorDefault, ColorBackground
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
                            kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString():null
                        );
                    }
                }
            }
            return null;
        }

        public void RemoveRestaurant(int restaurantId)
        {
            string sql = @"
        ALTER TABLE Restaurant
        SET isActive = @isActive
        WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@isActive", false);
                cmd.Parameters.AddWithValue("@Id", restaurantId);


                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
