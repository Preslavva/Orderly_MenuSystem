using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public partial class StaffRepository : Repository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration) { }
        public Staff GetStaffByEmailGlobal(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, FirstName, LastName, Email, Phone, IsActive, RestaurantId, Password, Salt
                        FROM Staff 
                        WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Staff(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["FirstName"]),
                                    Convert.ToString(reader["LastName"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToBoolean(reader["IsActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"])
                                );
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting staff by email: {sqlEx.Message}", sqlEx);
            }
        }
        public Staff GetStaffByEmail(string email, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, FirstName, LastName, Email, Phone, IsActive, RestaurantId, Password, Salt
                        FROM Staff 
                        WHERE Email = @Email AND RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Staff(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["FirstName"]),
                                    Convert.ToString(reader["LastName"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToBoolean(reader["IsActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"])
                                );
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting staff by email: {sqlEx.Message}", sqlEx);
            }
        }
        
        
        public Staff GetStaffById(int staffId, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, FirstName, LastName, Email, Phone, IsActive, RestaurantId, Password, Salt
                        FROM Staff 
                        WHERE Id = @Id AND RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", staffId);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Staff(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["FirstName"]),
                                    Convert.ToString(reader["LastName"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToBoolean(reader["IsActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"])
                                );
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting staff by ID: {sqlEx.Message}", sqlEx);
            }
        }
        public List<Staff> GetAllStaffIncludingInactive(int restaurantId)
        {
            List<Staff> staffList = new List<Staff>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, FirstName, LastName, Email, Phone, IsActive, RestaurantId
                        FROM Staff 
                        WHERE RestaurantId = @RestaurantId
                        ORDER BY FirstName, LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                staffList.Add(new Staff(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["FirstName"]),
                                    Convert.ToString(reader["LastName"]),
                                    Convert.ToString(reader["Email"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToBoolean(reader["IsActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    "", "" 
                                ));
                            }
                        }
                    }
                }
                return staffList;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting all staff: {sqlEx.Message}", sqlEx);
            }
        }
        public List<Staff> GetActiveStaff(int restaurantId)
        {
            return GetAllStaffIncludingInactive(restaurantId).Where(s => s.IsActive).ToList();
        }
        public int CreateStaff(Staff staff)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO Staff (FirstName, LastName, Email, Phone, IsActive, RestaurantId, Password, Salt)
                        OUTPUT INSERTED.Id
                        VALUES (@FirstName, @LastName, @Email, @Phone, @IsActive, @RestaurantId, @Password, @Salt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", staff.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", staff.LastName);
                        cmd.Parameters.AddWithValue("@Email", staff.Email);
                        cmd.Parameters.AddWithValue("@Phone", staff.Phone);
                        cmd.Parameters.AddWithValue("@IsActive", staff.IsActive);
                        cmd.Parameters.AddWithValue("@RestaurantId", staff.RestaurantId);
                        cmd.Parameters.AddWithValue("@Password", staff.Password);
                        cmd.Parameters.AddWithValue("@Salt", staff.Salt);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while creating staff: {sqlEx.Message}", sqlEx);
            }
        }
        public bool UpdateStaff(int staffId, string firstName, string lastName, string email, string phone, bool isActive, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Staff 
                        SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                            Phone = @Phone, IsActive = @IsActive
                        WHERE Id = @Id AND RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", staffId);
                        cmd.Parameters.AddWithValue("@FirstName", firstName ?? "");
                        cmd.Parameters.AddWithValue("@LastName", lastName ?? "");
                        cmd.Parameters.AddWithValue("@Email", email ?? "");
                        cmd.Parameters.AddWithValue("@Phone", phone ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updating staff: {sqlEx.Message}", sqlEx);
            }
        }
        public bool DeleteStaff(int staffId, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string deleteRolesQuery = @"
                                DELETE sr FROM StaffRole sr
                                INNER JOIN Staff s ON sr.StaffId = s.Id
                                WHERE s.Id = @StaffId AND s.RestaurantId = @RestaurantId";

                            using (SqlCommand cmd = new SqlCommand(deleteRolesQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@StaffId", staffId);
                                cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                                cmd.ExecuteNonQuery();
                            }
                            string deleteStaffQuery = "DELETE FROM Staff WHERE Id = @Id AND RestaurantId = @RestaurantId";
                            using (SqlCommand cmd = new SqlCommand(deleteStaffQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", staffId);
                                cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                                var result = cmd.ExecuteNonQuery() > 0;
                                
                                transaction.Commit();
                                return result;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while deleting staff: {sqlEx.Message}", sqlEx);
            }
        }
        public bool UpdateStaffPassword(int staffId, string hashedPassword, string salt, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Staff 
                        SET Password = @Password, Salt = @Salt
                        WHERE Id = @Id AND RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", staffId);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Salt", salt);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updating staff password: {sqlEx.Message}", sqlEx);
            }
        }
        public List<Role> GetStaffRoles(int staffId, int restaurantId)
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT r.Id, r.Type, r.RestaurantId 
                        FROM Role r
                        INNER JOIN StaffRole sr ON r.Id = sr.RoleId
                        INNER JOIN Staff s ON sr.StaffId = s.Id
                        WHERE sr.StaffId = @StaffId AND sr.isActive = 1 AND s.RestaurantId = @RestaurantId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StaffId", staffId);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(new Role(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Type"]),
                                    Convert.ToInt32(reader["RestaurantId"])
                                ));
                            }
                        }
                    }
                }
                return roles;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting staff roles: {sqlEx.Message}", sqlEx);
            }
        }
        public void AssignRoleToStaff(int staffId, int roleId, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string verifyQuery = @"
                        SELECT COUNT(*) FROM Staff s, Role r 
                        WHERE s.Id = @StaffId AND r.Id = @RoleId 
                        AND s.RestaurantId = @RestaurantId AND r.RestaurantId = @RestaurantId";

                    using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn))
                    {
                        verifyCmd.Parameters.AddWithValue("@StaffId", staffId);
                        verifyCmd.Parameters.AddWithValue("@RoleId", roleId);
                        verifyCmd.Parameters.AddWithValue("@RestaurantId", restaurantId);

                        if ((int)verifyCmd.ExecuteScalar() == 0)
                        {
                            throw new Exception("Staff or Role does not belong to this restaurant.");
                        }
                    }
                    string checkQuery = "SELECT COUNT(*) FROM StaffRole WHERE StaffId = @StaffId AND RoleId = @RoleId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@StaffId", staffId);
                        checkCmd.Parameters.AddWithValue("@RoleId", roleId);
                        
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            string updateQuery = "UPDATE StaffRole SET isActive = 1 WHERE StaffId = @StaffId AND RoleId = @RoleId";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@StaffId", staffId);
                                updateCmd.Parameters.AddWithValue("@RoleId", roleId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string insertQuery = @"
                                INSERT INTO StaffRole (StaffId, RoleId, isActive)
                                VALUES (@StaffId, @RoleId, 1)";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@StaffId", staffId);
                                insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while assigning role: {sqlEx.Message}", sqlEx);
            }
        }
        public void RemoveRoleFromStaff(int staffId, int roleId, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    string query = @"
                        UPDATE sr SET isActive = 0
                        FROM StaffRole sr
                        INNER JOIN Staff s ON sr.StaffId = s.Id
                        INNER JOIN Role r ON sr.RoleId = r.Id
                        WHERE sr.StaffId = @StaffId AND sr.RoleId = @RoleId 
                        AND s.RestaurantId = @RestaurantId AND r.RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StaffId", staffId);
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while removing role: {sqlEx.Message}", sqlEx);
            }
        }
        
        
    }
}