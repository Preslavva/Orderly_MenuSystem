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

        public void AddNutrition(string name, decimal value)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryAddNutrition = @"insert into Nutrition([Name], Value)
                                                 values(@Name, @Value)";

                    using (SqlCommand addNutrition = new SqlCommand(queryAddNutrition, conn))
                    {
                        addNutrition.Parameters.AddWithValue("@Name", name);
                        addNutrition.Parameters.AddWithValue("@Value", value);

                        addNutrition.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while adding customer: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
            }
        }

        public void AssignNutritionToMenuItem(string menuItemName, string nutritionName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string getMenuItemIdQuery = "SELECT Id FROM MenuItem WHERE Name = @MenuItemName;";
                    int? menuItemId = null;
                    using (SqlCommand cmd = new SqlCommand(getMenuItemIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemName", menuItemName);
                        object result = cmd.ExecuteScalar();
                        if (result != null) menuItemId = (int?)result;
                    }

                    string getNutritionIdQuery = "SELECT Id FROM Nutrition WHERE Name = @NutritionName;";
                    int? nutritionId = null;
                    using (SqlCommand cmd = new SqlCommand(getNutritionIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NutritionName", nutritionName);
                        object result = cmd.ExecuteScalar();
                        if (result != null) nutritionId = (int?)result;
                    }

                    if (menuItemId == null || nutritionId == null)
                    {
                        throw new Exception("MenuItem or Nutrition not found.");

                    }

                    string assignQuery = "INSERT INTO MenuItem_Nutrition (MenuItemId, NutritionId) VALUES (@MenuItemId, @NutritionId);";
                    using (SqlCommand cmd = new SqlCommand(assignQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId.Value);
                        cmd.Parameters.AddWithValue("@NutritionId", nutritionId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while assigning nutrition: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
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
                                    Value = reader.GetDecimal(2)
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


        public List<Nutrition>? GetNutritionsForMenuItem(int id)
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
                    WHERE mn.MenuItemId = @MenuItemId;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nutritions.Add(new Nutrition
                                {
                                    Id = reader.GetInt32(0),
                                    Name = (NutritionName)Enum.Parse(typeof(NutritionName), reader.GetString(1)),
                                    Value = reader.IsDBNull(2) ? 0.00m : reader.GetDecimal(2)


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
    }
}
