using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class TableRepository : Repository
    {
        public TableRepository(IConfiguration configuration) : base(configuration) { }

        public void CreateAddTableDB(Table table, int restaurantId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            INSERT INTO [Table] (QrCode, RestaurantId, GuidToken) 
                            VALUES (@QrCode, @RestaurantId, @GuidToken)
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@QrCode", table.QrCode);
                    cmdAddTable.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmdAddTable.Parameters.AddWithValue("@GuidToken", table.GuidToken);

                    cmdAddTable.ExecuteNonQuery();
                }
            }
        }

        public void CreateTableWithNumber(Table table, int restaurantId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            INSERT INTO [Table] (QrCode, RestaurantId, GuidToken, TableNumber) 
                            VALUES (@QrCode, @RestaurantId, @GuidToken, @TableNumber)
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@QrCode", table.QrCode);
                    cmdAddTable.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    cmdAddTable.Parameters.AddWithValue("@GuidToken", table.GuidToken);
                    cmdAddTable.Parameters.AddWithValue("@TableNumber", table.Number);

                    cmdAddTable.ExecuteNonQuery();
                }
            }
        }

        public byte[] GetTableQRById(int tableId, int restaurantId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            SELECT QrCode FROM [Table]
                            WHERE Id = @TableId AND RestaurantId = @RestaurantId
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@TableId", tableId);
                    cmdAddTable.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    var result = cmdAddTable.ExecuteScalar();

                    byte[] QrCode = result as byte[];

                    return QrCode;
                }
            }
        }

        public int GetTableNumberById(int tableId, int restaurantId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            SELECT TableNumber FROM [Table]
                            WHERE Id = @TableId AND RestaurantId = @RestaurantId
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@TableId", tableId);
                    cmdAddTable.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    var result = Convert.ToInt32(cmdAddTable.ExecuteScalar());
                    return result;
                }
            }
        }

        public List<Table> GetAllTables(int restaurantId)
        {
            var tables = new List<Table>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryGetTables = """
                            SELECT * FROM [Table] WHERE RestaurantId = @RestaurantId
                        """;

                using (SqlCommand cmdGetTables = new SqlCommand(queryGetTables, conn))
                {
                    cmdGetTables.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    SqlDataReader dr = cmdGetTables.ExecuteReader();

                    while (dr.Read())
                    {
                        tables.Add(new Table(Convert.ToInt32(dr["Id"]), (byte[])dr["QrCode"], dr["GuidToken"].ToString()));
                    }
                }
            }

            return tables;
        }

        public Table? GetTableByToken(string token, int restaurantId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryGetTable = """
                            SELECT * FROM [Table]
                            WHERE GuidToken = @GuidToken AND RestaurantId = @RestaurantId
                        """;

                using (SqlCommand cmdGetTable = new SqlCommand(queryGetTable, conn))
                {
                    cmdGetTable.Parameters.AddWithValue("@GuidToken", token);
                    cmdGetTable.Parameters.AddWithValue("@RestaurantId", restaurantId);

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
                    string query = @"SELECT Id, QrCode, TableNumber
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
                                    (byte[])reader["QrCode"],
                                    Convert.ToInt32(reader["TableNumber"])
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