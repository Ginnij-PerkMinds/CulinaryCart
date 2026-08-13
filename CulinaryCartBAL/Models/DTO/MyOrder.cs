namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class MyOrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal FinalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string? AppliedPromoCode { get; set; }
        public string? Remarks { get; set; }
        public string? RefundStatus { get; set; }
        public string? RefundImage { get; set; }
        public string? RefundUserRemarks { get; set; }
        public List<MyOrderItemDto> OrderItems { get; set; } = new();
    }

    public class MyOrderDetailsDto : MyOrderDto
    {
        public decimal BaseAmount { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal HandlingFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public List<MyOrderItemDto> OrderItems { get; set; } = new();
    }

    public class MyOrderItemDto
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
    }
}

