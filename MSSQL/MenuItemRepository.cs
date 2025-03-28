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

        public void AddMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryAddMenuItem = @"insert into MenuItem([Name], [Description], Price, IsAvailable, Picture, Category)
                                                 values(@Name, @Description, @Price, @IsAvailable, @Picture, @Cont)";

                    using (SqlCommand addMenuItem = new SqlCommand(queryAddMenuItem, conn))
                    {
                        addMenuItem.Parameters.AddWithValue("@Name", name);
                        addMenuItem.Parameters.AddWithValue("@Description", description);
                        addMenuItem.Parameters.AddWithValue("@Price", price);
                        addMenuItem.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        addMenuItem.Parameters.AddWithValue("@Picture", picture);
                        addMenuItem.Parameters.AddWithValue("@Category", category);


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
                    string queryGetMenuItems = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category
                                         FROM MenuItem";

                    using (SqlCommand getMenuItems = new SqlCommand(queryGetMenuItems, conn))
                    {
                        using (SqlDataReader reader = getMenuItems.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MenuItem item = new MenuItem(
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetDecimal(3),
                                    reader.GetBoolean(4),
                                    reader.GetString(5),
                                    (Category)Enum.Parse(typeof(Category), reader.GetString(6))
                                );

                                menuItems.Add(item);
                            }
                        }
                    }
                }

                return menuItems;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading menu items: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
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
                    string query = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category 
                             FROM MenuItem WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new MenuItem(
                                    reader.GetInt32(0),             
                                    reader.GetString(1),                 
                                    reader.GetString(2),                 
                                    reader.GetDecimal(3),               
                                    reader.GetBoolean(4),                
                                    reader.GetString(5),                 
                                    (Category)Enum.Parse(typeof(Category), reader.GetString(6)) 
                                );
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving MenuItem by ID: {ex.Message}", ex);
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
