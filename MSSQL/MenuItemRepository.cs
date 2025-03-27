using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Configuration;
using Models.Enums;
using Microsoft.Extensions.Configuration;
using Models.Entities;
namespace MSSQL
{
    public class MenuItemRepository : Repository
    {
        public MenuItemRepository(IConfiguration configuration) : base(configuration) { }

        public void AddMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category continent)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryAddMenuItem = @"insert into MenuItem([Name], [Description], Price, IsAvailable, Picture, Continent)
                                                 values(@Name, @Description, @Price, @IsAvailable, @Picture, @Continent)";

                    using (SqlCommand addMenuItem = new SqlCommand(queryAddMenuItem, conn))
                    {
                        addMenuItem.Parameters.AddWithValue("@Name", name);
                        addMenuItem.Parameters.AddWithValue("@Description", description);
                        addMenuItem.Parameters.AddWithValue("@Price", price);
                        addMenuItem.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        addMenuItem.Parameters.AddWithValue("@Picture", picture);
                        addMenuItem.Parameters.AddWithValue("@Continent", continent);


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
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
            }
        }

        public List<MenuItem>? LoadMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryGetCustomers = @"select Id, [Name], [Description], Price, IsAvailable, Picture, Category
                                                from MenuItem";

                    using (SqlCommand getCustomers = new SqlCommand(queryGetCustomers, conn))
                    {
                        SqlDataReader reader = getCustomers.ExecuteReader();

                        while (reader.Read())
                        {
                            string continentValue = Convert.ToString(reader["Category"]);
                            Category categoryEnum = (Category)Enum.Parse(typeof(Category), continentValue);

                            menuItems.Add(new MenuItem(

                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Name"]),
                                    Convert.ToString(reader["Description"]),
                                    Convert.ToDecimal(reader["Price"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
                                    Convert.ToString(reader["Picture"]),
                                     categoryEnum
                            ));                           
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
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex); //this could be remove
            }
        }

        public void DeleteMenuItem(MenuItem menuItem)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
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
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
            }
        }

        public MenuItem? GetMenuItemById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM MenuItem WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string continentValue = Convert.ToString(reader["Category"]);
                                Category categoryEnum = (Category)Enum.Parse(typeof(Category), continentValue);

                                return new MenuItem(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Name"]),
                                    Convert.ToString(reader["Description"]),
                                    Convert.ToDecimal(reader["Price"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
                                    Convert.ToString(reader["Picture"]),
                                    categoryEnum
 
                                    
                                );
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
        public void ChangeMenuItemAvailability(MenuItem menuItem, bool isAvailable)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
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
        public void UpdateMenuItemQuantity(MenuItem menuItem, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
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


    }
}
