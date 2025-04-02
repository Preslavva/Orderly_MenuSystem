using Models.Enums;

namespace Models.Entities
{
    public class Order
    {
        public int Id { get; }

        public int RestaurantId { get;}
        public Table Table { get; }
        public DateTime OrderTimestamp { get;    }
        public OrderStatus Status { get; }
        public int Quantity { get; }
        public decimal SubTotal { get; }
        public List<OrderItem> Items { get; private set; }

        public Order(int id, Table table, DateTime orderTimestamp, OrderStatus status, int restaurantId)
        {
            Id = id;
            Table = table;
            OrderTimestamp = orderTimestamp;
            Status = status;
            Items = new List<OrderItem>();
            RestaurantId = restaurantId;
        }

        public Order(Table table, DateTime orderTimestamp, OrderStatus status, int restaurantId)
        {
            Table = table;
            OrderTimestamp = orderTimestamp;
            Status = status;
            Items = new List<OrderItem>();
            RestaurantId = RestaurantId;
        }

        public void SetMenuItems(List<OrderItem> items)
        {
            Items = items;
        }
    }
}
