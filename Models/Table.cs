namespace OrderlyTest.Models
{
    public class Table
    {
        public int Id { get; }
        public string QrCode { get; }

        public Table(int id, string qrCode)
        {
            this.Id = id;
            this.QrCode = qrCode;
        }

        public Table(string qrCode)
        {
            this.QrCode=qrCode;
        }

    }
}
