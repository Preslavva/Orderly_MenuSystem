using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class StaffRepository : Repository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Staff GetStaffByEmail(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query =
                        @"SELECT Id, FirstName, LastName, Email, Phone, isActive, RestaurantId, Password, Salt 
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
    }
}