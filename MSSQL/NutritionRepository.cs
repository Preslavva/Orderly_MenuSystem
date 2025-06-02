using Microsoft.Data.SqlClient;
using System.Reflection;
using Models.Enums;
using Microsoft.Extensions.Configuration;
using Models.Entities;
namespace MSSQL
{
    public class NutritionRepository : Repository
    {
        public NutritionRepository(IConfiguration configuration) : base(configuration) { }

        public int AddNutrition(string name, int value)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO Nutrition ([Name], Value)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Value);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Value", value);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding nutrition: {ex.Message}", ex);
            }
        }

        public void AssignNutritionToMenuItem(int menuItemId, int nutritionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine($"🧪 Linking NutritionId={nutritionId} to MenuItemId={menuItemId}");

                string query = @"INSERT INTO MenuItem_Nutrition (MenuItemId, NutritionId)
                         VALUES (@MenuItemId, @NutritionId);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    cmd.Parameters.AddWithValue("@NutritionId", nutritionId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Nutrition> GetAllNutritions()
        {
            List<Nutrition> nutritions = new List<Nutrition>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Nutrition;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nutritions.Add(new Nutrition
                                {
                                    Id = reader.GetInt32(0),
                                    Name = (NutritionName)Enum.Parse(typeof(NutritionName), reader.GetString(1)),
                                    Value = reader.GetInt32(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving all Nutritions: {ex.Message}", ex);
            }

            return nutritions;
        }

        public List<Nutrition>? GetNutritionsForMenuItem(int id, int restaurantId)
        {
            List<Nutrition> nutritions = new List<Nutrition>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT n.Id, n.Name, n.Value
                    FROM Nutrition n
                    INNER JOIN MenuItem_Nutrition mn ON n.Id = mn.NutritionId
                    INNER JOIN MenuItem m ON mn.MenuItemId = m.Id
                    WHERE mn.MenuItemId = @MenuItemId AND m.RestaurantId = @RestaurantId;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", id);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nutritions.Add(new Nutrition
                                {
                                    Id = reader.GetInt32(0),
                                    Name = (NutritionName)Enum.Parse(typeof(NutritionName), reader.GetString(1)),
                                    Value =  reader.GetInt32(2)
                                });
                            }
                        }
                    }
                }

                return nutritions;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading customers: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
            }
        }

        public void DeleteAllForMenuItem(int menuItemId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string deleteLinks = "DELETE FROM MenuItem_Nutrition WHERE MenuItemId = @MenuItemId";
                    using (SqlCommand cmd = new SqlCommand(deleteLinks, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                        cmd.ExecuteNonQuery();
                    }

                    string deleteNutrition = @"
                DELETE FROM Nutrition
                WHERE Id NOT IN (SELECT NutritionId FROM MenuItem_Nutrition)";
                    using (SqlCommand cmd = new SqlCommand(deleteNutrition, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting nutritions for MenuItem {menuItemId}: {ex.Message}", ex);
            }
        }
    }
}