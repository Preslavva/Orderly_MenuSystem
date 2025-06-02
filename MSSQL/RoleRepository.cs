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

        public List<Role> GetAllRoles(int restaurantId)
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Type, RestaurantId FROM Role WHERE RestaurantId = @RestaurantId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                throw new Exception($"Database error occurred while getting roles: {sqlEx.Message}", sqlEx);
            }
        }

        public Role GetRoleByType(string type, int restaurantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Type, RestaurantId FROM Role WHERE Type = @Type AND RestaurantId = @RestaurantId;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Role(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Type"]),
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
                throw new Exception($"Database error occurred while getting role: {sqlEx.Message}", sqlEx);
            }
        }
    }
}