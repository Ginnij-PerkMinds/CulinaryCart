namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    using System.Collections.Generic;

    namespace CulinaryCart.CulinaryCartBAL.DTOs
    {
        public class CartItemDto
        {
            public int FoodItemId { get; set; }
            public string FoodItemName { get; set; }
            public int Quantity { get; set; }
            public decimal FinalPrice { get; set; }
        }
        public class CartChargeDto
        {
            public string ChargeType { get; set; }
            public decimal Value { get; set; }
        }
        public class CartResponseDto
        {
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
            public decimal BaseAmount { get; set; }
            public decimal PromoDiscount { get; set; }
            public List<CartChargeDto> Charges { get; set; } = new List<CartChargeDto>();
            public decimal FinalAmount { get; set; }
            public string Message { get; set; }
            public string AppliedPromoCode { get; set; }
        }
        public class CartCalculationResult
        {
            public decimal BaseAmount { get; set; }
            public decimal PromoDiscount { get; set; }
            public string AppliedPromoCode { get; set; }
            public decimal HandlingFee { get; set; }
            public decimal DeliveryFee { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal FinalAmount { get; set; }
        }
    }
}