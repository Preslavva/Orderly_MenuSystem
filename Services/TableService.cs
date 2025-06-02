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

        public void CreateAddTableDB(Table table, int restaurantId)
        {
            _tableRepository.CreateAddTableDB(table, restaurantId);
        }

        public void CreateTableWithNumber(Table table, int restaurantId)
        {
            _tableRepository.CreateTableWithNumber(table, restaurantId);
        }
        public void CreateTableWitNumber(Table table, int restaurantId)
        {
            _tableRepository.CreateTableWithNumber(table, restaurantId);
        }

        public byte[] GetTableQRById(int tableId, int restaurantId)
        {
            var QrCode = _tableRepository.GetTableQRById(tableId, restaurantId);
             
            if(QrCode == null)
            {
                return null;
            }

            return QrCode;
        }

        public int? GetTableNumberById(int tableId, int restaurantId)
        {
            var tableNum = _tableRepository.GetTableNumberById(tableId, restaurantId);

            if(tableNum == null)
            {
                return null;
            }

            return tableNum;
        }

        public List<Table> GetAllTables(int restaurantId)
        {
            return _tableRepository.GetAllTables(restaurantId);
        }

        public Table GetTableByToken(string token, int restaurantId)
        {
            return _tableRepository.GetTableByToken(token, restaurantId);
        }

        public List<Table> GetTablesByRestaurantId(int restaurantId)
        {
            return _tableRepository.GetTablesByRestaurantId(restaurantId);
        }
    }
}