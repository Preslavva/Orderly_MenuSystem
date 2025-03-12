namespace OrderlyTest.Models
{
    public class Table
    {
        public int Id { get; set; }
        public byte[] QrCode { get; set; }

        public Table(int id, byte[] qrCode)
        {
            Id = id;
            QrCode = qrCode;
        }


    }
}
