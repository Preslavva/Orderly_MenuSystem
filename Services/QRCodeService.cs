using QRCoder;

namespace Services
{
    public class QRCodeService
    {
        public byte[] GenerateQRCode(string url)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                return qrCode.GetGraphic(15);
            }
        }
    }
}
