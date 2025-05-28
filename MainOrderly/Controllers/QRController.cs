using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;
using MainOrderly.WebApp.Extensions;

namespace MainOrderly.WebApp.Controllers
{
    public class QRController : Controller
    {
        private readonly string _baseUrl;
        private readonly QRCodeService _qrCodeService;
        private readonly TableService _tableService;

        public QRController(QRCodeService qrCodeService, TableService tableService, IConfiguration configuration)
        {
            _qrCodeService = qrCodeService;
            _tableService = tableService;
            _baseUrl = configuration["AppSettings:BaseUrl"]!;
        }

        public IActionResult GenerateAndStoreTables()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            int restaurantId = user?.RestaurantId ?? 1;
         
            var tables = new List<Table>();

            for (int i = 1; i <= 3; i++)
            {
                string guidToken = Guid.NewGuid().ToString();
                string qrUrl = $"{_baseUrl}/Home/LoadingPage?token={guidToken}";
                byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrUrl);

                var table = new Table(qrCodeImage, guidToken, i);
                
                _tableService.CreateAddTableDB(table, restaurantId);
                tables.Add(table);
            }

            return RedirectToAction("QrView");
        }

        public IActionResult QrView()
        {
            var user = HttpContext.Session.GetAuthenticatedUser();
            int restaurantId = user?.RestaurantId ?? 1;
                         
            List<Table> tables = _tableService.GetTablesByRestaurantId(restaurantId);
            return View(tables);
        }
    }
}