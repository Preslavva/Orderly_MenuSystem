using Models.Entities;
using MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class WaiterService
    {
        private readonly WaiterRepository _waiterRepository;

        public WaiterService(WaiterRepository waiterRepository)
        {
            _waiterRepository = waiterRepository;
        }

        public List<Order> GetCompletedOrdersWithItems()
        {
            return _waiterRepository.GetCompletedOrdersWithItems();
        }

        public void UpdateOrderStatusDelivered(int orderId)
        {
            _waiterRepository.UpdateOrderStatusDelivered(orderId);
        }

    }
}
