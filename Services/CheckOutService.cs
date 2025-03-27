using Models.Enums;
using MSSQL;
using Models.Entities;

public class CheckoutService
{
    private readonly CartRepository _cartRepository;
    private readonly KitchenOrderRepository _kitchenOrderRepository;

    public CheckoutService(CartRepository cartRepository, KitchenOrderRepository kitchenOrderRepository)
    {
        _cartRepository = cartRepository;
        _kitchenOrderRepository = kitchenOrderRepository;   
    }

    public int FinalizeOrder(int tableId, Dictionary<Models.Entities.MenuItem, int> cartItems)
    {
        int totalQuantity = 0;
        decimal totalPrice = 0;
        foreach (var kvp in cartItems)
        {
            totalQuantity += kvp.Value;
            totalPrice += kvp.Key.Price * kvp.Value;
        }

        int newOrderId = _kitchenOrderRepository.CreateOrder(tableId, OrderStatus.NEW_ORDER, totalQuantity, totalPrice);

        foreach (var kvp in cartItems)
        {
           _cartRepository.AddMenuItemToOrder(newOrderId, kvp.Key.Id, kvp.Value);
        }

        return newOrderId;
    }
}
