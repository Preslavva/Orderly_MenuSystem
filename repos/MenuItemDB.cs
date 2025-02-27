using OrderlyTest.Models;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Configuration;
namespace OrderlyTest.repos
{
    public class MenuItemDB
    {
        private readonly string _connectionString;

        public MenuItemDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public static void AddMenuItem(string connectionString, string name, string description, decimal price, bool isAvailable, string picture)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryAddMenuItem = @"insert into MenuItem([Name], [Description], Price, IsAvailable, Picture)
                                                 values(@Name, @Description, @Price, @IsAvailable, @Picture)";

                    using (SqlCommand addMenuItem = new SqlCommand(queryAddMenuItem, conn))
                    {
                        addMenuItem.Parameters.AddWithValue("@Name", name);
                        addMenuItem.Parameters.AddWithValue("@Description", description);
                        addMenuItem.Parameters.AddWithValue("@Price", price);
                        addMenuItem.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        addMenuItem.Parameters.AddWithValue("@Picture", isAvailable);

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

        protected static List<MenuItem>? LoadMenuItems(string connectionString)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryGetCustomers = @"select Id, [Name], [Description], Price, IsAvailable, Picture
                                                from MenuItem";

                    using (SqlCommand getCustomers = new SqlCommand(queryGetCustomers, conn))
                    {
                        SqlDataReader reader = getCustomers.ExecuteReader();

                        while (reader.Read())
                        {
                            menuItems.Add(new MenuItem(
                                (int)reader["Id"],
                                reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                (decimal)reader["Price"],
                                (bool)reader["IsAvailable"],
                                reader["Picture"].ToString()));
                        }
                    }
                    return menuItems;
                }
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
