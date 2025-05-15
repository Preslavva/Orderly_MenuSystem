using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using MSSQL;

namespace Services
{
    public class HistoryService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly OrderHistoryRepository _historyRepository;
        public HistoryService(IHttpContextAccessor contextAccessor, OrderHistoryRepository historyRepository)
        {
            _contextAccessor = contextAccessor;
            _historyRepository = historyRepository;
        }

        public void SaveOrderIds(int newOrderId)
        {
            List<int> orderIdsHistory = DeserializeHistory();

            orderIdsHistory.Add(newOrderId);

           SerializeHistory(orderIdsHistory);

        }

        public List<OrderHistory> GetHistory()
        {
            List<int>historyIds = DeserializeHistory();
            List<OrderHistory> history = new List<OrderHistory>();
            foreach (var historyId in historyIds)
            {
               history.AddRange(_historyRepository.GetHistoryOrders(historyId));
            }
            return history.OrderByDescending(h => h.OrderTimeStamp).ToList();
        }

        private List<int> DeserializeHistory()
        {
            string existing = _contextAccessor.HttpContext.Request.Cookies["OrderHistory"];
            List<int> history = new List<int>();
            if (!string.IsNullOrEmpty(existing))
            {
                try
                {
                    history = JsonSerializer.Deserialize<List<int>>(existing) ?? new List<int>();
                }
                catch
                {
                    history = new List<int>();
                }
            }
            return history;
        }

        private void SerializeHistory(List<int> orderIdsHistory)
        {
            string updatedOrderList = JsonSerializer.Serialize(orderIdsHistory);
            _contextAccessor.HttpContext.Response.Cookies.Append("OrderHistory", updatedOrderList, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(5),
                IsEssential = true,
                HttpOnly = true
            });
        }

    }
}
