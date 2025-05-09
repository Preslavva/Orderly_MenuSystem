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

        public void CreateTableWitNumber(Table table)
        {
            _tableRepository.CreateTableWithNumber(table);
        }

        public byte[] GetTableQRById(int tableId)
        {
            var QrCode = _tableRepository.GetTableQRById(tableId);
             
            if(QrCode == null)
            {
                return null;
            }

            return QrCode;
        }

        public int? GetTableNumberById(int tableId)
        {
            var tableNum = _tableRepository.GetTableNumberById(tableId);

            if(tableNum == null)
            {
                return null;
            }

            return tableNum;
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
