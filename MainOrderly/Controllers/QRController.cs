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
         
            var tables = new List<Table>();

            for (int i = 1; i <= 3; i++)
            {
                string guidToken = Guid.NewGuid().ToString();
                string qrUrl = $"{_baseUrl}/Home/LoadingPage?token={guidToken}";
                byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrUrl);

                var table = new Table(i, qrCodeImage, guidToken);
                _tableService.CreateAddTableDB(table);

                tables.Add(table);
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