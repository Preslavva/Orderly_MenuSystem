using Models.Entities;
using MSSQL;

namespace Services
{
    public class TableService
    {
        private readonly TableRepository _tableRepository;

        public TableService(TableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public void CreateAddTableDB(Table table)
        {
            _tableRepository.CreateAddTableDB(table);
        }

        public List<Table> GetAllTables()
        {
            return _tableRepository.GetAllTables();
        }

        public Table GetTableByToken(string token)
        {
            return _tableRepository.GetTableByToken(token);
        }

        public List<Table> GetTablesByRestaurantId(int restaurantId)
        {
            return _tableRepository.GetTablesByRestaurantId(restaurantId);
        }
    }
}
