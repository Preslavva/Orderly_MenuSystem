using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Entities;


namespace MainOrderly.WebApp.Controllers
{
    public class QRController : Controller
    {
        private readonly string _baseUrl; //test
        private readonly QRCodeService _qrCodeService;
        private readonly TableService _tableService;
        public QRController(QRCodeService qrCodeService, TableService tableService, IConfiguration configuration)
        {
            _qrCodeService = qrCodeService;
            _tableService = tableService;
            _baseUrl = configuration["AppSettings:BaseUrl"]!; //test

        }

        public IActionResult GenerateAndStoreTables()
        {
            var tables = new[]
         {
            //new Table(1, _qrCodeService.GenerateQRCode($"https://localhost:5053/Home/Index?tableId=1")),
            //new Table(2, _qrCodeService.GenerateQRCode($"https://localhost:5053/Home/Index?tableId=2")),
            //new Table(3, _qrCodeService.GenerateQRCode($"https://localhost:5053/Home/Index?tableId=3")),
            //new Table(4, _qrCodeService.GenerateQRCode($"https://localhost:5053/Home/Index?tableId=4")),
            //new Table(5, _qrCodeService.GenerateQRCode($"{_baseUrl}/Home/Index?tableId=5")),
            //new Table(6, _qrCodeService.GenerateQRCode($"{_baseUrl}/Home/Index?tableId=6")),
            new Table(1, _qrCodeService.GenerateQRCode($"{_baseUrl}/Home/LoadingPage?tableId=1")),
            new Table(2, _qrCodeService.GenerateQRCode($"{_baseUrl}/Home/LoadingPage?tableId=2")),


        };

            foreach (Table table in tables)
            {
                _tableService.CreateAddTableDB(table);
            }

            return RedirectToAction("QrView");

        }

        public IActionResult QrView()
        {
            List<Table> tables = _tableService.GetAllTables();
            return View(tables);
        }
    }
}