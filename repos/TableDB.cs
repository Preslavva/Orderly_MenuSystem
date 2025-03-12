using Microsoft.Data.SqlClient;
using OrderlyTest.Models;

namespace Orderly.repos
{
    public class TableDB
    {
        private readonly string _connectionString;

        public TableDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public void CreateAddTableDB(Table table)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryAddTable = """
                            INSERT INTO Table(Id, QrCode) 
                            VALUES (@Id, @QrCode)
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
                    cmdAddTable.Parameters.AddWithValue("@Id", table.Id);
                    cmdAddTable.Parameters.AddWithValue("@QrCode", table.QrCode); ;

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
                            SELECT * FROM Table
                        """;

                using (SqlCommand cmdGetTables = new SqlCommand(queryGetTables, conn))
                {
                    SqlDataReader dr = cmdGetTables.ExecuteReader();

                    while(dr.Read())
                    {
                        tables.Add(new Table(Convert.ToInt32(dr["Id"]), (byte[])dr["QrCode"]));
                    }
                }
            }

            return tables;
        }


    }
}
