namespace OrderlyTest.Models
{
    public class OrderItem
    {
        public int Id { get;}
        public MenuItem MenuItem { get; set; }

        public Order Order { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }

        public string Picture { get; set; }

        public OrderItem(int id, MenuItem menuItem, int quantity, decimal subTotal, string picture)
        {
            this.Id = id;
            this.MenuItem = menuItem;
            this.Quantity = quantity;
            this.SubTotal = subTotal;
            this.Picture = picture;
        }
        public OrderItem(MenuItem menuItem, int quantity, decimal subTotal)
        {
            this.MenuItem = menuItem;
            this.Quantity = quantity;
            this.SubTotal = subTotal;
        }
        public OrderItem() { }
    }
}
