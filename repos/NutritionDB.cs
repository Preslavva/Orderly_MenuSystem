using OrderlyTest.Models;
using Microsoft.Data.SqlClient;
using System.Reflection;
namespace OrderlyTest.repos
{
    public class NutritionDB
    {
        private readonly string _connectionString;

        public NutritionDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        protected static void AddNutrition(string connectionString, string name, decimal value)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryAddNutrition = @"insert into Nutrition([Name], Value)
                                                 values(@Name, @Value)";

                    using (SqlCommand addMenuItem = new SqlCommand(queryAddNutrition, conn))
                    {
                        addMenuItem.Parameters.AddWithValue("@Name", name);
                        addMenuItem.Parameters.AddWithValue("@Value", value);

                        addMenuItem.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while adding customer: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public static void AssignNutritionToMenuItem(string connectionString, string menuItemName, string nutritionName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                        return;
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
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        protected static List<Nutrition>? GetNutritionsForMenuItem(string connectionString, MenuItem menuItem)
        {
            List<Nutrition> nutritions = new List<Nutrition>();

            try
            {

                string query = @"
        SELECT n.Id, n.Name, n.Value
        FROM Nutrition n
        INNER JOIN MenuItem_Nutrition mn ON n.Id = mn.NutritionId
        WHERE mn.MenuItemId = @MenuItemId;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItem.Id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nutritions.Add(new Nutrition
                                {
                                    Id = reader.GetInt32(0),
                                    Name = (NutritionName)Enum.Parse(typeof(NutritionName),reader.GetString(1)),
                                    Value = reader.GetDecimal(2)

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
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }
    }
}
