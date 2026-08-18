namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Refund
    {
        public int RefundId { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundStatus { get; set; } = "Pending";
        public string? Remarks { get; set; }

        // NEW: user-facing fields
        public string? RefundImage { get; set; }
        public string? RefundUserRemarks { get; set; }

        // Navigation
        public Order Order { get; set; }
        public User User { get; set; }

        public ICollection<RefundItem> RefundItems { get; set; } = new List<RefundItem>();
    }

    public class RefundItem
    {
        public int RefundItemId { get; set; }
        public int RefundId { get; set; }
        public int FoodItemID { get; set; }
        public string? RefundImage { get; set; }
        public string? Remarks { get; set; }

        // Navigation properties
        public Refund Refund { get; set; }
        public Menu Menu { get; set; }
    }
}

