namespace OrderlyTest.Models
{
    public class Order
    {
        public int Id { get; }
        public  int  TableId { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; }

        public Order(int id, int tableId, DateTime orderTimestamp, OrderStatus status)
        {
            this.Id = id;
            this.TableId = tableId;
            this.OrderTimestamp = orderTimestamp;
            this.Status = status;
            this.Items= new List<OrderItem>();
        }

        public Order(int tableId, DateTime orderTimestamp, OrderStatus status)
        {
            this.TableId = tableId;
            this.OrderTimestamp = orderTimestamp;
            this.Status = status;
            this.Items = new List<OrderItem>();
        }
    }
}
