namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Promocode
    {
        public int Id { get; set; }
        public string PromoCodeName { get; set; }
        public decimal? Amount { get; set; }
        public decimal Criteria { get; set; }
        public bool FreeDelivery { get; set; }
        public int? UsageCount { get; set; }
        public bool IsActive { get; set; }
    }
}