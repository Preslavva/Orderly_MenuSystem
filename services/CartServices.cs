using OrderlyTest.Models;

namespace OrderlyTest.services
{
    public class CartServices
    {
        private readonly IHttpContextAccessor contextAccessor;
        public List<MenuItem> menuItems;
        private int counter;

        public CartServices(IHttpContextAccessor contxtAccessor)
        {
            this.contextAccessor = contxtAccessor;
            menuItems = new List<MenuItem>();
            counter = 0;
        }

        public void AddToCart()
        {
            GetCartCount();
            counter++;
            contextAccessor.HttpContext?.Session.SetInt32("CartCount",counter);
        }

        public int GetCartCount()
        {
            int counter = contextAccessor.HttpContext?.Session.GetInt32("CartCount") ?? 0;
            return counter;
        }
    }
}
