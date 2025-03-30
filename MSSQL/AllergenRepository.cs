using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enums;

namespace MSSQL
{
    public class AllergenRepository : Repository
    {
        public AllergenRepository(IConfiguration configuration) : base(configuration) { }

        public List<Allergen>? GetAllergensForMenuItem(int id)
        {
            List<Allergen> allergens = new List<Allergen>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                   SELECT a.Id, a.Name
FROM Allergen a
INNER JOIN MenuItem_Allergen ma ON a.Id = ma.AllergenId
WHERE ma.MenuItemId  = @MenuItemId;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allergens.Add(new Allergen(
                               (int)reader["Id"],
                               (AllergenName)Enum.Parse(typeof(AllergenName), reader.GetString(1))
 ));

                            }
                        }
                    }
                }

                return allergens;
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
    }
}