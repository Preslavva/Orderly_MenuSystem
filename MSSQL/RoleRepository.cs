using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class RoleRepository : Repository
    {
        public RoleRepository(IConfiguration configuration) : base(configuration) { }

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Type FROM Role;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                throw new Exception($"Database error occurred while getting roles: {sqlEx.Message}", sqlEx);
            }
        }

        public Role GetRoleByType(string type)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Type FROM Role WHERE Type = @Type;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Type", type);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Role(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Type"])
                                );
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while getting role: {sqlEx.Message}", sqlEx);
            }
        }
    }
}
