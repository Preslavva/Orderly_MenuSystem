using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class OwnerRepository : Repository
    {
        public OwnerRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Owner GetOwnerByEmail(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, FirstName, LastName, Email, Phone, Password, Salt, RestaurantId 
                                    FROM Owner WHERE Email = @Email;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Owner(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["FirstName"]),
                                    Convert.ToString(reader["LastName"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"]),
                                    Convert.ToInt32(reader["RestaurantId"])
                                );
                            }
                        }
                    }
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting owner: {sqlEx.Message}", sqlEx);
            }
        }

        public void AssignRestaurantToOwner(int ownerId, int restaurantId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);  
                connection.Open();

                string sql = @"UPDATE Owner 
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
    }
}