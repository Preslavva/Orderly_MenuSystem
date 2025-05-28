using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class RestaurantRepository: Repository
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration) { }

        public int CreateRestaurant(Restaurant restaurant)
        {

            string sql = @"
                INSERT INTO Restaurant([Name], Email, Phone, [Address], Description, KVK, isActive, Logo)
                VALUES(@Name, @Email, @Phone, @Address, @Description, @KVK, @isActive, @Logo);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

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
                cmd.Parameters.AddWithValue("@Logo", restaurant.Logo);

                conn.Open();
                int insertedId = (int)cmd.ExecuteScalar();
                return insertedId;
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
               Logo, Font, ColorButtons, ColorDefault, ColorBackground, KVK
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

        public Restaurant GetRestaurantByKVK(string KVK)
        {
            using SqlConnection _connection = new SqlConnection(_connectionString);
            _connection.Open();

            string sql = @" SELECT Id, [Name], Email, Phone, [Address], Description, 
                                 Logo, Font, ColorButtons, ColorDefault, ColorBackground, KVK
                                 FROM Restaurant
                                 WHERE KVK = @KVK";
            using SqlCommand command = new SqlCommand(sql, _connection);
            command.Parameters.AddWithValue("@KVK", KVK);

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
						   kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString() : null
					   );
			}
            return null;
        }

        public void AssignRestaurantToOwner(int ownerId, int restaurantId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = @"UPDATE Staff 
                                 SET RestaurantId = @RestaurantId 
                                 WHERE Id = @OwnerId";
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);
                command.Parameters.AddWithValue("@OwnerId", ownerId);

                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred assigning restaurant to owner: {sqlEx.Message}", sqlEx);
            }
        }

        public Restaurant GetOwnerRestaurant(int? ownerId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = @"Select r.* 
                                from Restaurant as r
                                inner join Staff as s
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
                           kvk: reader["KVK"] != DBNull.Value ? reader["KVK"].ToString() : null
                       );
                }
                return null;

            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred assigning restaurant to owner: {sqlEx.Message}", sqlEx);
            }
        }

        public bool DoesKvkExist(Restaurant restaurant)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = @"Select Count(*) 
                                from Restaurant
                                where KVK = @Kvk";
                if (restaurant.Id != null || restaurant.Id != 0)
                    sql += " and Id <> @id";

                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Kvk", restaurant.KVK);
                if (restaurant.Id != null || restaurant.Id != 0)
                    command.Parameters.AddWithValue("@id", restaurant.Id);

                int count = (int)command.ExecuteScalar();
                return count > 0;

            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred assigning restaurant to owner: {sqlEx.Message}", sqlEx);
            }
        }
    }
}
