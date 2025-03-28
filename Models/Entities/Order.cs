using Models.Enums;

namespace Models.Entities
{
    public class Order
    {
        public int Id { get; }
        public Table Table { get; }
        public DateTime OrderTimestamp { get;    }
        public OrderStatus Status { get; }
        public int Quantity { get; }
        public decimal SubTotal { get; }
        public List<MenuItem> Items { get; private set; }

        public Order(int id, Table table, DateTime orderTimestamp, OrderStatus status)
        {
            Id = id;
            Table = table;
            OrderTimestamp = orderTimestamp;
            Status = status;
            Items = new List<MenuItem>();
        }

        public Order(Table table, DateTime orderTimestamp, OrderStatus status)
        {
            Table = table;
            OrderTimestamp = orderTimestamp;
            Status = status;
            Items = new List<MenuItem>();
        }

        public void SetMenuItems(List<MenuItem> items)
        {
            Items = items;
        }
    }
}
