using Models.Enums;

namespace Models.Entities
{
    public class OrderHistory
    {
        public int Id { get;  }
        public string Name { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        public decimal SubTotal { get; }
        public OrderStatus Status { get; }

        public OrderHistory(int id, string name, int quantity, decimal price, decimal subtotal, OrderStatus status)
        {
            this.Id = id;   
            this.Name = name;
            this.Quantity = quantity;               
            this.Price = price;
            this.SubTotal = subtotal;
            this.Status = status;
        }
    }
}
