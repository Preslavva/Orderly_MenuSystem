namespace Models.Entities
{
    public class Table
    {
        public int Id { get; }
        public byte[] QrCode { get; set; }
        public string GuidToken { get; set; }


        public Table(int id, byte[] qrCode, string guidToken)
        {
            Id = id;
            QrCode = qrCode;
            GuidToken = guidToken;
        }

        public Table(byte[] qrCode, string guidToken)
        {
            QrCode = qrCode;
            GuidToken = guidToken;
        }

        public Table(int id, byte[] qrCode)
        {
            Id = id;
            QrCode = qrCode;
        }

    }
}
