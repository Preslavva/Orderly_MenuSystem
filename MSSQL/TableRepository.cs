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
                            INSERT INTO [Table] (QrCode, RestaurantId) 
                            VALUES (@QrCode, 1)
                        """;

                using (SqlCommand cmdAddTable = new SqlCommand(queryAddTable, conn))
                {
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
                            SELECT * FROM [Table]
                        """;

                using (SqlCommand cmdGetTables = new SqlCommand(queryGetTables, conn))
                {
                    SqlDataReader dr = cmdGetTables.ExecuteReader();

                    while (dr.Read())
                    {
                        tables.Add(new Table(Convert.ToInt32(dr["Id"]), (byte[])dr["QrCode"]));
                    }
                }
            }

            return tables;
        }


    }
}