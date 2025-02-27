using Microsoft.AspNetCore.Mvc;
using OrderlyTest.services;

namespace OrderlyTest.Controllers
{
    public class QRController : Controller
    {
        private readonly QRCodeService _qrCodeService;

        public QRController()
        {
            _qrCodeService = new QRCodeService();
        }

        public IActionResult Generate(int tableId)
        {
            string url = $"{Request.Scheme}://{Request.Host}/Order/Menu?tableId={tableId}";
            byte[] qrCodeImage = _qrCodeService.GenerateQRCode(url); 
            return File(qrCodeImage, "image/png"); 
        }

        public IActionResult QrView()
        {
            return View();
        }
    }
}
