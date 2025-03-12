using OrderlyTest.Models;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Configuration;
namespace OrderlyTest.repos
{
    public class MenuItemDB
    {/*
        private const string connectionString = "Server=mssqlstud.fhict.local;Database=dbi547761;User Id=dbi547761s;Password=12345; Encrypt=True;TrustServerCertificate=True;";
        private readonly string _connectionString;

        public MenuItemDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public void AddMenuItem(string name, string description, decimal price, bool isAvailable, string picture)
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

        public List<MenuItem>? LoadMenuItems()
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

        protected static void DeleteMenuItem(string connectionString,MenuItem menuItem)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryDeleteMenuItem = @"delete from MenuItem
                                                   where Id = @Id";

                    using (SqlCommand deleteMenuItem = new SqlCommand(queryDeleteMenuItem, conn))
                    {
                        deleteMenuItem.Parameters.AddWithValue("@Id", menuItem.Id);

                        deleteMenuItem.ExecuteNonQuery();
                        
                    }
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

        public static MenuItem? GetMenuItemById(string connectionString, MenuItem menuItem)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM MenuItem WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", menuItem.Id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new MenuItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    IsAvailable = reader.GetBoolean(4),
                                    Picture = reader.GetString(5)
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving MenuItem: {ex.Message}", ex);
            }
        }
        public static void ChangeMenuItemAvailability(string connectionString, MenuItem menuItem, bool isAvailable)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE MenuItem SET IsAvailable = @IsAvailable WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", menuItem.Id);
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating MenuItem availability: {ex.Message}", ex);
            }
        }
        public static void UpdateMenuItemQuantity(string connectionString, MenuItem menuItem, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE MenuItem SET Quantity = @Quantity WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", menuItem.Id);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating MenuItem quantity: {ex.Message}", ex);
            }
        }

*/
    }
}
