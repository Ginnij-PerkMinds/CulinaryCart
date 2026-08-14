namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    // Basic refund info
    public class RefundDto
    {
        public int RefundId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundStatus { get; set; }
        public string? Remarks { get; set; }
        public string? RefundImage { get; set; }         
        public string? RefundUserRemarks { get; set; }
    }

    // Detailed refund info
    public class RefundDetailsDto : RefundDto
    {
        public int OrderId { get; set; }   // link back to original order
        public decimal BaseAmount { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal HandlingFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class RefundClaimDto
    {
        public int OrderId { get; set; }
        public int? ItemId { get; set; } // null if "all items"
        public string? Remarks { get; set; }
        public IFormFile? ProofFile { get; set; }
        public decimal RefundAmount { get; set; }
    }

    public class RejectRefundDto
    {
        public string Remarks { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; } = 0;
    }

    public class   AcceptRefundDto
    {
        public string Remarks { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
    }
}
