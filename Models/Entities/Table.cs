namespace Models.Entities
{
    public class Table
    {
        public int Id { get; }
        public byte[] QrCode { get; }


        public Table(int id, byte[] qrCode)
        {
            Id = id;
            QrCode = qrCode;
        }


    }
}
