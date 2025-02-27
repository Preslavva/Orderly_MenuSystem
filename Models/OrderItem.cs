namespace OrderlyTest.Models
{
    public class OrderItem
    {
        public int Id { get;}
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }

        public OrderItem(int id, MenuItem menuItem, int quantity, decimal subTotal)
        {
            this.Id = id;
            this.MenuItem = menuItem;
            this.Quantity = quantity;
            this.SubTotal = subTotal;
        }
        public OrderItem(MenuItem menuItem, int quantity, decimal subTotal)
        {
            this.MenuItem = menuItem;
            this.Quantity = quantity;
            this.SubTotal = subTotal;
        }
    }
}
