using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class StaffRepository : Repository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration) { }

        public Staff GetStaffByEmail(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt 
                                    FROM Staff WHERE Email = @Email;";

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
                                    Convert.ToBoolean(reader["isActive"]),
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
                throw new Exception($"Database error occurred while getting staff: {sqlEx.Message}", sqlEx);
            }
        }

        public Staff GetStaffById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt 
                                    FROM Staff WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
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
                                    Convert.ToBoolean(reader["isActive"]),
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
                throw new Exception($"Database error occurred while getting staff: {sqlEx.Message}", sqlEx);
            }
        }

        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt 
                           FROM Staff 
                           WHERE isActive = 1;"; 

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                                    Convert.ToBoolean(reader["isActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"])
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
        public List<Staff> GetAllStaffIncludingInactive()
        {
            List<Staff> staffList = new List<Staff>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt 
                           FROM Staff;"; 

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                                    Convert.ToBoolean(reader["isActive"]),
                                    Convert.ToInt32(reader["RestaurantId"]),
                                    Convert.ToString(reader["Password"]),
                                    Convert.ToString(reader["Salt"])
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

        public int CreateStaff(Staff staff)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Staff (FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt)
                                    OUTPUT INSERTED.Id
                                    VALUES (@FirstName, @LastName, @Email, @Phone, @IsActive, @RestaurantId, @Password, @Salt);";

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

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while creating staff: {sqlEx.Message}", sqlEx);
            }
        }

        public void AssignRoleToStaff(int staffId, int roleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // First check if the staff-role relationship already exists
                    string checkQuery = @"SELECT COUNT(*) FROM StaffRole 
                                        WHERE StaffId = @StaffId AND RoleId = @RoleId;";
                    
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@StaffId", staffId);
                        checkCmd.Parameters.AddWithValue("@RoleId", roleId);
                        
                        int exists = (int)checkCmd.ExecuteScalar();
                        
                        if (exists > 0)
                        {
                            // Update existing record
                            string updateQuery = @"UPDATE StaffRole 
                                                SET isActive = 1 
                                                WHERE StaffId = @StaffId AND RoleId = @RoleId;";
                            
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@StaffId", staffId);
                                updateCmd.Parameters.AddWithValue("@RoleId", roleId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Insert new record
                            string insertQuery = @"INSERT INTO StaffRole (StaffId, RoleId, isActive)
                                                VALUES (@StaffId, @RoleId, 1);";
                            
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
                throw new Exception($"Database error occurred while assigning role to staff: {sqlEx.Message}", sqlEx);
            }
        }

        public List<Role> GetStaffRoles(int staffId)
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT r.Id, r.Type 
                                    FROM Role r
                                    INNER JOIN StaffRole sr ON r.Id = sr.RoleId
                                    WHERE sr.StaffId = @StaffId AND sr.isActive = 1;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StaffId", staffId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(new Role(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Type"])
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
        
        public bool UpdateStaff(int id, string firstName, string lastName, string email, string phone, bool isActive)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Staff 
                            SET FirstName = @FirstName, 
                                LastName = @LastName, 
                                Email = @Email, 
                                Phone = @Phone, 
                                isActive = @IsActive 
                            WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updating staff: {sqlEx.Message}", sqlEx);
            }
        }

        public bool RemoveRoleFromStaff(int staffId, int roleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE StaffRole 
                                    SET isActive = 0 
                                    WHERE StaffId = @StaffId AND RoleId = @RoleId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StaffId", staffId);
                        cmd.Parameters.AddWithValue("@RoleId", roleId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while removing role from staff: {sqlEx.Message}", sqlEx);
            }
        }
        
        public bool DeleteStaff(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // First, deactivate all roles for this staff
                            string deactivateRolesQuery = @"UPDATE StaffRole 
                                              SET isActive = 0 
                                              WHERE StaffId = @StaffId;";
                                              
                            using (SqlCommand cmd = new SqlCommand(deactivateRolesQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@StaffId", id);
                                cmd.ExecuteNonQuery();
                            }
                            
                            // Now, truly delete the staff record
                            string deleteStaffQuery = @"DELETE FROM Staff 
                                             WHERE Id = @Id;";
                                             
                            using (SqlCommand cmd = new SqlCommand(deleteStaffQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                
                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    return true;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Transaction failed: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while deleting staff: {sqlEx.Message}", sqlEx);
            }
        }
    }
}
