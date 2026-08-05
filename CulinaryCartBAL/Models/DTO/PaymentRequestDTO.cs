namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class PaymentRequestDto
    {
        public decimal FinalAmount { get; set; }
        //public string Currency { get; set; } = "INR";
        public string PromoCode { get; set; }
    }

    public class PaymentVerificationDto
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
        public string PromoCode { get; set; }
    }
}