using System;
using System.Collections.Concurrent;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Helpers
{
    public class TimerHelpers
    {
        private readonly MenuService _menuService;

        // Static dictionary to store timer start times per order.
        private static readonly ConcurrentDictionary<int, DateTime> _orderStartTimes = new ConcurrentDictionary<int, DateTime>();

        public TimerHelpers(MenuService menuService)
        {
            _menuService = menuService;
        }

        // Records the start time for an order.
        public void RecordStartTime(int orderId)
        {
            _orderStartTimes.TryAdd(orderId, DateTime.Now);
        }

        // Retrieves the elapsed time for an order.
        public string GetElapsedTime(int orderId)
        {
            if (_orderStartTimes.TryGetValue(orderId, out DateTime startTime))
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                return $"{elapsed.Minutes:00}:{elapsed.Seconds:00}";
            }
            return "00:00";
        }

        // Checks whether the order's elapsed time has exceeded the average prep time.
        // You can adjust this method depending on how you want to calculate the expected prep time.
        public bool HasExceededPrepTime(int orderId)
        {
            if (_orderStartTimes.TryGetValue(orderId, out DateTime startTime))
            {
                var avgOrderPrepTime = _menuService.CalculateOrderPrepTime(orderId);
                TimeSpan elapsed = DateTime.Now - startTime;
                return elapsed.TotalMinutes >= avgOrderPrepTime;
            }
            return false;
        }

        // Removes the timer for an order (e.g. when completed).
        public void RemoveTimer(int orderId)
        {
            _orderStartTimes.TryRemove(orderId, out _);
        }
    }
}