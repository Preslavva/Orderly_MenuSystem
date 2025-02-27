using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OrderlyTest.services
{
    public class QRCodeService
    {
        public byte[] GenerateQRCode(string url)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())  // QR Code generator
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q); // Create QR code data
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData); // Generates QR as PNG bytes
                return qrCode.GetGraphic(20); // Returns the PNG byte array
            }
        }
    }
}
