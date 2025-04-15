using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL
{
    public class IngredientRepository: Repository
    {
        public IngredientRepository(IConfiguration configuration) : base(configuration) { }

        public Ingredient GetIngredientById(int id)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, Name, Unit, QuantityInStock, MinimumStockLevel, RestaurantId FROM Ingredient WHERE Id = @Id;";

                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Id", id);
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return new Ingredient(

                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Name"])!,
                                    Convert.ToString(reader["Unit"])!,
                                    Convert.ToInt32(reader["QuantityInStock"]),
                                    Convert.ToInt32(reader["MinimumStockLevel"]),
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
                throw new Exception($"Database error occurred while getting ingredients items: {sqlEx.Message}", sqlEx);
            }
        }

        public List<Ingredient> GetIngredientsByRestaurantId(int restaurantId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, Name, Unit, QuantityInStock, MinimumStockLevel, RestaurantId 
                               FROM Ingredient WHERE RestaurantId = @RestaurantId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ingredients.Add(new Ingredient(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Name"])!,
                                    Convert.ToString(reader["Unit"])!,
                                    Convert.ToInt32(reader["QuantityInStock"]),
                                    Convert.ToInt32(reader["MinimumStockLevel"]),
                                    Convert.ToInt32(reader["RestaurantId"])
                                ));
                            }
                        }
                    }
                }
                return ingredients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving Ingredients for Restaurant: {ex.Message}", ex);
            }
        }

        public int AddIngredient(Ingredient ingredient)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Ingredient (Name, Unit, QuantityInStock, MinimumStockLevel, RestaurantId)
                               OUTPUT INSERTED.Id
                               VALUES (@Name, @Unit, @QuantityInStock, @MinimumStockLevel, @RestaurantId);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", ingredient.Name);
                        cmd.Parameters.AddWithValue("@Unit", ingredient.Unit);
                        cmd.Parameters.AddWithValue("@QuantityInStock", ingredient.QuantityInStock);
                        cmd.Parameters.AddWithValue("@MinimumStockLevel", ingredient.MinimumStockLevel);
                        cmd.Parameters.AddWithValue("@RestaurantId", ingredient.RestaurantId);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding Ingredient: {ex.Message}", ex);
            }
        }

        public void UpdateIngredientStock(int id, decimal quantityInStock)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Ingredient SET QuantityInStock = @QuantityInStock WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@QuantityInStock", quantityInStock);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating Ingredient stock: {ex.Message}", ex);
            }
        }

      

        public List<MenuItemIngredient> GetIngredientsForMenuItem(int menuItemId)
        {
            List<MenuItemIngredient> menuItemIngredients = new List<MenuItemIngredient>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT mi.MenuItemId, mi.IngredientId, mi.Quantity, 
                        i.Name, i.Unit, i.QuantityInStock, i.MinimumStockLevel, i.RestaurantId
                        FROM MenuItem_Ingredient mi
                        INNER JOIN Ingredient i ON mi.IngredientId = i.Id
                        WHERE mi.MenuItemId = @MenuItemId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Ingredient ingredient = new Ingredient(
                                    Convert.ToInt32(reader["IngredientId"]),
                                    Convert.ToString(reader["Name"])!,
                                    Convert.ToString(reader["Unit"])!,
                                    Convert.ToInt32(reader["QuantityInStock"]),
                                    Convert.ToInt32(reader["MinimumStockLevel"]),
                                    Convert.ToInt32(reader["RestaurantId"])
                                );

                                menuItemIngredients.Add(new MenuItemIngredient(
                                    Convert.ToInt32(reader["MenuItemId"]),
                                    Convert.ToInt32(reader["IngredientId"]),
                                    ingredient,
                                    Convert.ToInt32(reader["Quantity"])
                                ));
                            }
                        }
                    }
                }
                return menuItemIngredients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving Ingredients for MenuItem: {ex.Message}", ex);
            }
        }

        public void AddIngredientToMenuItem(int menuItemId, int ingredientId, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO MenuItem_Ingredient (MenuItemId, IngredientId, Quantity)
                        VALUES (@MenuItemId, @IngredientId, @Quantity);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientId", ingredientId);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding Ingredient to MenuItem: {ex.Message}", ex);
            }
        }

        public void RemoveIngredientFromMenuItem(int menuItemId, int ingredientId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        DELETE FROM MenuItem_Ingredient 
                        WHERE MenuItemId = @MenuItemId AND IngredientId = @IngredientId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientId", ingredientId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while removing Ingredient from MenuItem: {ex.Message}", ex);
            }
        }

        public void UpdateIngredientForMenuItem(int menuItemId, int ingredientId, decimal quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE MenuItem_Ingredient 
                        SET Quantity = @Quantity
                        WHERE MenuItemId = @MenuItemId AND IngredientId = @IngredientId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                        cmd.Parameters.AddWithValue("@IngredientId", ingredientId);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating Ingredient for MenuItem: {ex.Message}", ex);
            }
        }
        public bool UpdateMenuItemIngredients(int menuItemId, Dictionary<int, decimal> ingredientQuantities)
        {
            if (menuItemId <= 0)
                throw new ArgumentException("Invalid MenuItem ID.");

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            var deleteCmd = new SqlCommand(
                                "DELETE FROM MenuItem_Ingredient WHERE MenuItemId = @MenuItemId", conn, transaction);
                            deleteCmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                            deleteCmd.ExecuteNonQuery();

                            if (ingredientQuantities != null && ingredientQuantities.Any())
                            {
                                foreach (var kvp in ingredientQuantities)
                                {
                                    var insertCmd = new SqlCommand(@"
                                INSERT INTO MenuItem_Ingredient (MenuItemId, IngredientId, Quantity)
                                VALUES (@MenuItemId, @IngredientId, @Quantity)", conn, transaction);

                                    insertCmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                                    insertCmd.Parameters.AddWithValue("@IngredientId", kvp.Key);
                                    insertCmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = kvp.Value;

                                    insertCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Failed to update ingredients. Transaction rolled back: " + ex.Message, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating MenuItem ingredients: {ex.Message}", ex);
            }
        }


        public List<Ingredient> GetIngredientsForItem(int menuItemId)
        {
            List<Ingredient> Ingredients = new List<Ingredient>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                                    select [Name] from Ingredient as i
                                    inner join MenuItem_Ingredient as mi
                                    on i.Id = mi.IngredientId
                                    where mi.MenuItemId = @menuId
                                    ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@menuId", menuItemId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Ingredients.Add( new Ingredient(
                                    Convert.ToString(reader["Name"])!
                                ));
                            }
                        }
                    }
                }
                return Ingredients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving Ingredients for MenuItem: {ex.Message}", ex);
            }
        }
    }
}


    

