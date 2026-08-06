using CulinaryCart.CulinaryCartBAL.Models.DTO.CulinaryCart.CulinaryCartBAL.DTOs;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class PaymentRequestDto
    {
        public decimal BaseAmount { get; set; }
        public decimal PromoDiscount { get; set; }
        public string AppliedPromoCode { get; set; }
        public decimal HandlingFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Currency { get; set; } = "INR";
    }

    public class PaymentVerificationDto
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
        public string PromoCode { get; set; }
    }
    public class PaymentOrderResponseDto
    {
        public string RazorpayOrderId { get; set; }
        public int Amount { get; set; } // paise
        public PaymentRequestDto Dto { get; set; }
        public List<CartItemDto> Items { get; set; }
    }


    public class RazorpayOrderResponse
    {
        public string id { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
    }
}