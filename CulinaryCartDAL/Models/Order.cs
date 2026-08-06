namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }

        // Status: InCart, CheckedOut, etc.
        public string Status { get; set; } = "CheckedOut";

        // Legacy field (kept for compatibility)
        public decimal TotalAmount { get; set; }

        // 🔹 New fields for breakdown
        public decimal BaseAmount { get; set; }          // Sum of item final prices
        public decimal PromoDiscount { get; set; }       // Discount applied from promo code
        public decimal HandlingFee { get; set; }         // Handling fee applied
        public decimal DeliveryFee { get; set; }         // Delivery fee applied
        public decimal TaxAmount { get; set; }           // Taxes applied (SGST/CGST or combined)
        public decimal FinalAmount { get; set; }         // Grand total payable

        public string? AppliedPromoCode { get; set; }    // Track which promo code was used

        public string? RazorpayOrderId { get; set; }
        public string? PaymentId { get; set; }

        // Items in the order
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
    }
}