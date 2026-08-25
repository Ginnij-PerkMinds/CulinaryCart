namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class OtpEntry
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
    }
}

