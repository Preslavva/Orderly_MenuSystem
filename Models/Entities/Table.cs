namespace Models.Entities
{
    public class Table
    {
        public int Id { get; }
        public byte[] QrCode { get; set; }
        public string GuidToken { get; set; }
        public int Number { get; set; }

        public Table(byte[] qrCode, string guidToken, int tableNumber)
        {
            QrCode = qrCode;
            GuidToken = guidToken;
            Number = tableNumber;
        }

        public Table(int id,byte[] qrCode, int tableNumber)
        {
            Id = id;
            QrCode = qrCode;
            Number = tableNumber;
        }

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

        public Table(int id, int number)
        {
            Id = id;
            Number = number;
        }

    }
}
