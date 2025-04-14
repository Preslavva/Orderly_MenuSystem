using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Configuration;
using Models.Enums;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using System.Diagnostics;
using System.Data;
namespace MSSQL
{
    public class MenuItemRepository : Repository
    {
        public MenuItemRepository(IConfiguration configuration) : base(configuration) { }

        public bool AddMenuIngredients(int menuId, int[] ingredientIds, int[] quantities)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = @"
                            INSERT INTO MenuItem_Ingredient (MenuItemId, IngredientId, Quantity)
                            VALUES (@MenuItemId, @IngredientId, @Quantity)";

                            cmd.Parameters.Add("@MenuItemId", SqlDbType.Int);
                            cmd.Parameters.Add("@IngredientId", SqlDbType.Int);
                            cmd.Parameters.Add("@Quantity", SqlDbType.Int);

                            for (int i = 0; i < ingredientIds.Length; i++)
                            {
                                cmd.Parameters["@MenuItemId"].Value = menuId;
                                cmd.Parameters["@IngredientId"].Value = ingredientIds[i];
                                cmd.Parameters["@Quantity"].Value = quantities[i];

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}", ex);
            }
        }
        public int AddMenuItem(string name, string description, decimal price, bool isAvailable, string picture, Category category, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
               INSERT INTO MenuItem ([Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId)
OUTPUT INSERTED.Id
VALUES (@Name, @Description, @Price, @IsAvailable, @Picture, @Category, @RestaurantId);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                        cmd.Parameters.AddWithValue("@Picture", picture);
                        cmd.Parameters.AddWithValue("@Category", category.ToString());
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                        object result = cmd.ExecuteScalar();
                        Console.WriteLine("ExecuteScalar raw result: " + (result ?? "null"));

                        if (result != null && result != DBNull.Value)
                        {
                            int id = Convert.ToInt32(result);
                            Console.WriteLine("menuId = " + id);
                            return id;
                        }
                        else
                        {
                            throw new Exception("No ID returned from ExecuteScalar()");
                        }



                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while adding menu item: {sqlEx.Message}", sqlEx);
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
                    string queryGetMenuItems = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId
                                         FROM MenuItem WHERE RestaurantId = @RestaurantId";

                    using (SqlCommand getMenuItems = new SqlCommand(queryGetMenuItems, conn))
                    {
                        getMenuItems.Parameters.AddWithValue("@RestaurantId", 1);
                        using (SqlDataReader reader = getMenuItems.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                string categoryValue = Convert.ToString(reader["Category"]);
                                Category categoryEnum = (Category)Enum.Parse(typeof(Category), categoryValue.Replace(" ", "_"));

                                menuItems.Add(new MenuItem(

                                        Convert.ToInt32(reader["Id"]),
                                        Convert.ToString(reader["Name"])!,
                                        Convert.ToString(reader["Description"])!,
                                        Convert.ToDecimal(reader["Price"]),
                                        Convert.ToBoolean(reader["IsAvailable"]),
                                        Convert.ToString(reader["Picture"])!,
                                        categoryEnum,
                                        Convert.ToInt32(reader["RestaurantId"])
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
        public MenuItem? GetMenuItemById(int id, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, [Name], [Description], Price, IsAvailable, Picture, Category, RestaurantId
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
                                    Convert.ToInt32(reader["RestaurantId"])


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
        public bool DeleteMenuItem(int menuItemId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM MenuItem WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", menuItemId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public void UpdateMenuItem(MenuItem menuItem)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                UPDATE MenuItem SET 
                    Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    IsAvailable = @IsAvailable,
                    Picture = @Picture,
                    Category = @Category,
                    RestaurantId = @RestaurantId
                WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", menuItem.Name);
                        cmd.Parameters.AddWithValue("@Description", menuItem.Description);
                        cmd.Parameters.AddWithValue("@Price", menuItem.Price);
                        cmd.Parameters.AddWithValue("@IsAvailable", menuItem.IsAvailable);
                        cmd.Parameters.AddWithValue("@Picture", menuItem.Picture);
                        cmd.Parameters.AddWithValue("@Category", menuItem.Category.ToString());
                        cmd.Parameters.AddWithValue("@RestaurantId", menuItem.RestaurantId);
                        cmd.Parameters.AddWithValue("@Id", menuItem.Id);

                        cmd.ExecuteNonQuery(); 
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updating menu item: {sqlEx.Message}", sqlEx);
            }
        }


    }
}
