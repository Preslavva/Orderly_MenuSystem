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

        public int AddMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    int newItemId = 0;
                    conn.Open();
                    string queryAddMenuItem = @"insert into MenuItem([Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId)
                                                 values(@Name, @Description, @Price, @IsAvailable, @Picture, @Category, @RestaurantId)";

                    using (SqlCommand cmd = new SqlCommand(queryAddMenuItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        cmd.Parameters.AddWithValue("@Picture", picture);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@RestaurantId",restaurantId);


                       newItemId  = (int)cmd.ExecuteScalar();
                    }
                    
                    return newItemId; // need to check this.
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

        public List<MenuItem>? LoadMenuItems(int restaurantId)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryGetMenuItems = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId, PrepTime
                                         FROM MenuItem WHERE RestaurantId = @RestaurantId";

                    using (SqlCommand getMenuItems = new SqlCommand(queryGetMenuItems, conn))
                    {
                        getMenuItems.Parameters.AddWithValue("@RestaurantId",1);
                        using (SqlDataReader reader = getMenuItems.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                string categoryValue = Convert.ToString(reader["Category"]);
                                Category categoryEnum = (Category)Enum.Parse(typeof(Category), categoryValue);

                                menuItems.Add(new MenuItem(

                                        Convert.ToInt32(reader["Id"]),
                                        Convert.ToString(reader["Name"])!,
                                        Convert.ToString(reader["Description"])!,
                                        Convert.ToDecimal(reader["Price"]),
                                        Convert.ToBoolean(reader["IsAvailable"]),
                                        Convert.ToString(reader["Picture"])!,
                                        categoryEnum,
                                        Convert.ToInt32(reader["RestaurantId"]),
                                        Convert.ToInt32(reader["PrepTime"])
                                ));                           
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

        public MenuItem? GetMenuItemById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId, PrepTime
                             FROM MenuItem WHERE Id = @Id AND RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@RestaurantId", 1);
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
                                    categoryEnum,
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToInt32(reader["PrepTime"])
 
                                    
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
