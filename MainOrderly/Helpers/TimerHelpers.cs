using System;
using System.Collections.Concurrent;
using Models.Entities;
using Services;

namespace MainOrderly.WebApp.Helpers
{
    public class TimerHelpers
    {
        private readonly MenuService _menuService;
        private readonly KitchenOrderService _kitchenOrderService;

        /* start time + max-prep-time cache -------------------------- */
        private static readonly ConcurrentDictionary<int, DateTime> _startTimes =
            new ConcurrentDictionary<int, DateTime>();

        private static readonly ConcurrentDictionary<int, int> _maxPrepMinutes =
            new ConcurrentDictionary<int, int>();   // e.g. 5 for the “steak+fries” example

        public TimerHelpers(MenuService menuService, KitchenOrderService kitchenOrderService)
        {
            _menuService = menuService;
            _kitchenOrderService = kitchenOrderService;
        }
            


        /* called when the order moves to PROCESSING ----------------- */
        public void RecordStartTime(int orderId)
        {
            _startTimes.TryAdd(orderId, DateTime.Now);

            /* cache the *largest* prep-time among this order’s items  */
            int maxPrep = _kitchenOrderService.GetMenuItemsByOrderId(orderId).Max(i => i.MenuItem.PrepTime);
            _maxPrepMinutes.AddOrUpdate(orderId, maxPrep, (_, __) => maxPrep);
        }

        /* ===== New: Remaining time (count-down) ==================== */
        public string GetRemainingTime(int orderId, out int secondsRemaining)
        {
            if (_startTimes.TryGetValue(orderId, out DateTime start) &&
                _maxPrepMinutes.TryGetValue(orderId, out int maxMinutes))
            {
                int totalSeconds = maxMinutes * 60;
                int elapsed = (int)(DateTime.Now - start).TotalSeconds;
                secondsRemaining = Math.Max(totalSeconds - elapsed, 0);

                var ts = TimeSpan.FromSeconds(secondsRemaining);
                return $"{ts.Minutes:00}:{ts.Seconds:00}";
            }

            secondsRemaining = 0;
            return "00:00";
        }

        /* keeps the red-flash logic exactly as before, but based on
           remaining time rather than elapsed ----------------------- */
        public bool HasExceededPrepTime(int orderId)
        {
            GetRemainingTime(orderId, out int secRemain);
            return secRemain == 0;
        }

        public void RemoveTimer(int orderId)
        {
            _startTimes.TryRemove(orderId, out _);
            _maxPrepMinutes.TryRemove(orderId, out _);
        }
    }
}
