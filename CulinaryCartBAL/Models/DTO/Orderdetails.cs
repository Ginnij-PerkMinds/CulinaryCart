namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    // Basic order info
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }   // flatten from User.Address.ToString()
        public string PhoneNo { get; set; }
        public decimal FinalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string AppliedPromoCode { get; set; }
        public string? Remarks { get; set; }
    }

    // Detailed order info
    public class OrderDetailsDto : OrderDto
    {
        public decimal BaseAmount { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal HandlingFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    // Items inside an order
    public class OrderItemDto
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
    }
}


