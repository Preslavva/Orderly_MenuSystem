using Models.Enums;

namespace Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public OrderStatus Status { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public List<MenuItem> Items { get; set; }

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
    }
}
