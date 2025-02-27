namespace OrderlyTest.Models
{
    public class Order
    {
        public int Id { get; }
        public  Table  Table { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; }

        public Order(int id, Table table, DateTime orderTimestamp, OrderStatus status)
        {
            this.Id = id;
            this.Table = table;
            this.OrderTimestamp = orderTimestamp;
            this.Status = status;
            this.Items= new List<OrderItem>();
        }

        public Order(Table table, DateTime orderTimestamp, OrderStatus status)
        {
            this.Table = table;
            this.OrderTimestamp = orderTimestamp;
            this.Status = status;
            this.Items = new List<OrderItem>();
        }
    }
}
