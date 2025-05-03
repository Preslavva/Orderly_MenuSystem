using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;


namespace MSSQL
{
    public class TableRepository : Repository
    {

        public TableRepository(IConfiguration configuration) : base(configuration) { }

        public void CreateAddTableDB(Table table)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            INSERT INTO [Table] (QrCode, RestaurantId, GuidToken) 
                            VALUES (@QrCode, 1, @GuidToken)
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@QrCode", table.QrCode);
                    cmdAddTable.Parameters.AddWithValue("@GuidToken", table.GuidToken);

                    cmdAddTable.ExecuteNonQuery();
                }
            }
        }

        public List<Table> GetAllTables()
        {
            var tables = new List<Table>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryGetTables = """
                            SELECT * FROM [Table]
                        """;

                using (SqlCommand cmdGetTables = new SqlCommand(queryGetTables, conn))
                {
                    SqlDataReader dr = cmdGetTables.ExecuteReader();

                    while (dr.Read())
                    {
                        tables.Add(new Table(Convert.ToInt32(dr["Id"]), (byte[])dr["QrCode"], dr["GuidToken"].ToString()));
                    }
                }
            }

            return tables;
        }

        public Table? GetTableByToken(string token)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryGetTable = """
                            SELECT * FROM [Table]
                            WHERE GuidToken = @GuidToken
                        """;

                using (SqlCommand cmdGetTable = new SqlCommand(queryGetTable, conn))
                {
                    cmdGetTable.Parameters.AddWithValue("@GuidToken", token);

                    SqlDataReader dr = cmdGetTable.ExecuteReader();

                    if(dr.Read())
                    {
                        return new Table(
                    Convert.ToInt32(dr["Id"]),
                    (byte[])dr["QRCode"],
                    dr["GuidToken"].ToString()
                );
                    }
                }
            }

            return null;
        }

        public List<Table> GetTablesByRestaurantId(int restaurantId)
        {
            List<Table> tables = new List<Table>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, QrCode
                               FROM [Table] WHERE RestaurantId = @RestaurantId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(new Table(
                                    Convert.ToInt32(reader["Id"]),
                                    (byte[])reader["QrCode"]
                                ));
                            }
                        }
                    }
                }
                return tables;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving Tables for Restaurant: {ex.Message}", ex);
            }
        }
    }
}