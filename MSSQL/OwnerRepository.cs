using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class OwnerRepository : Repository
    {
        public OwnerRepository(IConfiguration configuration) : base(configuration) { }

        public Owner GetByEmail(string email)
        {
            var query = @"
                SELECT o.*, r.Name as RestaurantName, r.Email as RestaurantEmail, 
                       r.Phone as RestaurantPhone, r.Address as RestaurantAddress, 
                       r.KVK, r.Description, r.isActive
                FROM Owner o 
                INNER JOIN Restaurant r ON o.RestaurantId = r.Id 
                WHERE o.Email = @Email";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Owner(
                                Convert.ToInt32(reader["Id"]),
                                Convert.ToString(reader["FirstName"]),
                                Convert.ToString(reader["LastName"]),
                                Convert.ToString(reader["Email"]),
                                Convert.ToString(reader["Phone"]),
                                Convert.ToString(reader["Password"]),
                                Convert.ToString(reader["Salt"]),
                                Convert.ToInt32(reader["RestaurantId"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        public Owner GetById(int id)
        {
            var query = @"
                SELECT o.*, r.Name as RestaurantName, r.Email as RestaurantEmail, 
                       r.Phone as RestaurantPhone, r.Address as RestaurantAddress, 
                       r.KVK, r.Description, r.isActive as RestaurantIsActive
                FROM Owner o 
                INNER JOIN Restaurant r ON o.RestaurantId = r.Id 
                WHERE o.Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Owner(
                                Convert.ToInt32(reader["Id"]),
                                Convert.ToString(reader["FirstName"]),
                                Convert.ToString(reader["LastName"]),
                                Convert.ToString(reader["Email"]),
                                Convert.ToString(reader["Phone"]),
                                Convert.ToString(reader["Password"]),
                                Convert.ToString(reader["Salt"]),
                                Convert.ToInt32(reader["RestaurantId"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        public bool IsKvkExists(string kvk)
        {
            var query = "SELECT COUNT(*) FROM Restaurant WHERE KVK = @KVK";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KVK", kvk);

                    connection.Open();
                    var count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool RegisterOwnerWithRestaurant(string firstName, string lastName, string email, string phone,
            string hashedPassword, string salt, string restaurantName, string restaurantEmail, 
            string restaurantPhone, string restaurantAddress, string kvk, string description)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var restaurantQuery = @"
                            INSERT INTO Restaurant (Name, Email, Phone, Address, KVK, Description, isActive,
                                                  ColorButtons, ColorDefault, ColorBackground, Font)
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Email, @Phone, @Address, @KVK, @Description, 1,
                                    '#28a745', '#ffffff', '#f8f9fa', 'Inter')";

                        int restaurantId;
                        using (SqlCommand restaurantCommand = new SqlCommand(restaurantQuery, connection, transaction))
                        {
                            restaurantCommand.Parameters.AddWithValue("@Name", restaurantName);
                            restaurantCommand.Parameters.AddWithValue("@Email", restaurantEmail);
                            restaurantCommand.Parameters.AddWithValue("@Phone", restaurantPhone);
                            restaurantCommand.Parameters.AddWithValue("@Address", restaurantAddress);
                            restaurantCommand.Parameters.AddWithValue("@KVK", kvk);
                            restaurantCommand.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);

                            restaurantId = (int)restaurantCommand.ExecuteScalar();
                        }
                        var ownerQuery = @"
                            INSERT INTO Owner (FirstName, LastName, Email, Phone, Password, Salt, RestaurantId)
                            VALUES (@FirstName, @LastName, @Email, @Phone, @Password, @Salt, @RestaurantId)";

                        using (SqlCommand ownerCommand = new SqlCommand(ownerQuery, connection, transaction))
                        {
                            ownerCommand.Parameters.AddWithValue("@FirstName", firstName);
                            ownerCommand.Parameters.AddWithValue("@LastName", lastName);
                            ownerCommand.Parameters.AddWithValue("@Email", email);
                            ownerCommand.Parameters.AddWithValue("@Phone", phone);
                            ownerCommand.Parameters.AddWithValue("@Password", hashedPassword);
                            ownerCommand.Parameters.AddWithValue("@Salt", salt);
                            ownerCommand.Parameters.AddWithValue("@RestaurantId", restaurantId);

                            ownerCommand.ExecuteNonQuery();
                        }
                        var defaultRoles = new List<string>
                        {
                            "Manager", "Chef", "Waiter"
                        };

                        foreach (var roleType in defaultRoles)
                        {
                            var roleQuery = @"
                                INSERT INTO Role (Type, RestaurantId)
                                VALUES (@Type, @RestaurantId)";

                            using (SqlCommand roleCommand = new SqlCommand(roleQuery, connection, transaction))
                            {
                                roleCommand.Parameters.AddWithValue("@Type", roleType);
                                roleCommand.Parameters.AddWithValue("@RestaurantId", restaurantId);
                                roleCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool UpdateOwner(int ownerId, string firstName, string lastName, string email, string phone)
        {
            var query = @"
                UPDATE Owner 
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone
                WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ownerId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateRestaurant(int restaurantId, string name, string email, string phone, 
            string address, string description)
        {
            var query = @"
                UPDATE Restaurant 
                SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, Description = @Description
                WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurantId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeactivateRestaurant(int restaurantId)
        {
            var query = "UPDATE Restaurant SET isActive = 0 WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurantId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        public Restaurant GetRestaurantByOwnerId(int ownerId)
        {
            var query = @"
                SELECT r.* FROM Restaurant r 
                INNER JOIN Owner o ON r.Id = o.RestaurantId 
                WHERE o.Id = @OwnerId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OwnerId", ownerId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Restaurant(
                                Convert.ToInt32(reader["Id"]),
                                Convert.ToString(reader["Name"]),
                                Convert.ToString(reader["Email"]),
                                Convert.ToString(reader["Phone"]),
                                Convert.ToString(reader["Address"]),
                                reader.IsDBNull("Description") ? null : Convert.ToString(reader["Description"]),
                                reader.IsDBNull("Logo") ? null : (byte[])reader["Logo"],
                                reader.IsDBNull("Font") ? null : Convert.ToString(reader["Font"]),
                                reader.IsDBNull("ColorButtons") ? null : Convert.ToString(reader["ColorButtons"]),
                                reader.IsDBNull("ColorDefault") ? null : Convert.ToString(reader["ColorDefault"]),
                                reader.IsDBNull("ColorBackground") ? null : Convert.ToString(reader["ColorBackground"]),
                                reader.IsDBNull("isActive") ? false : Convert.ToBoolean(reader["isActive"]),
                                reader.IsDBNull("KVK") ? null : Convert.ToString(reader["KVK"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdatePassword(int ownerId, string hashedPassword, string salt)
        {
            var query = @"
                UPDATE Owner 
                SET Password = @Password, Salt = @Salt
                WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ownerId);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Salt", salt);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateRestaurantStyling(int restaurantId, string colorButtons, string colorDefault, 
            string colorBackground, string font)
        {
            var query = @"
                UPDATE Restaurant 
                SET ColorButtons = @ColorButtons, ColorDefault = @ColorDefault, 
                    ColorBackground = @ColorBackground, Font = @Font
                WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurantId);
                    command.Parameters.AddWithValue("@ColorButtons", (object)colorButtons ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ColorDefault", (object)colorDefault ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ColorBackground", (object)colorBackground ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Font", (object)font ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool ActivateRestaurant(int restaurantId)
        {
            var query = "UPDATE Restaurant SET isActive = 1 WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurantId);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateRestaurantLogo(int restaurantId, byte[] logo)
        {
            var query = @"
                UPDATE Restaurant 
                SET Logo = @Logo
                WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurantId);
                    command.Parameters.AddWithValue("@Logo", (object)logo ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}