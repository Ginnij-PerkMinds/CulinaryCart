namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Refund
    {
        public int RefundId { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal FinalAmount { get; set; }
        public string RefundStatus { get; set; } = "Pending";
        public string? Remarks { get; set; }

        // Navigation
        public Order Order { get; set; }
        public User User { get; set; }
    }
}

