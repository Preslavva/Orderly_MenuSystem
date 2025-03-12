using Microsoft.AspNetCore.Mvc;
using Orderly.repos;
using OrderlyTest.Models;
using OrderlyTest.services;

namespace OrderlyTest.Controllers
{
    public class QRController : Controller
    {
        private readonly QRCodeService _qrCodeService;
        private readonly TableDB _tableDB;

        public QRController(QRCodeService qrCodeService, TableDB tableDB)
        {
            _qrCodeService = qrCodeService;
            _tableDB = tableDB;
        }

        public IActionResult GenerateAndStoreTables()
        {
            var tables = new[]
         {
            new Table(1, _qrCodeService.GenerateQRCode($"https://localhost:7196/Home/Index?tableId=1")),
            new Table(2, _qrCodeService.GenerateQRCode($"https://localhost:7196/Home/Index?tableId=2")),
            new Table(3, _qrCodeService.GenerateQRCode($"https://localhost:7196/Home/Index?tableId=3")),
            new Table(4, _qrCodeService.GenerateQRCode($"https://localhost:7196/Home/Index?tableId=4"))
        };

            foreach (var table in tables)
            {
                _tableDB.CreateAddTableDB(table);
            }

            return RedirectToAction("QrView");

        }

        public IActionResult QrView()
        {
            List<Table> tables = _tableDB.GetAllTables();
            return View(tables);
        }
    }
}
