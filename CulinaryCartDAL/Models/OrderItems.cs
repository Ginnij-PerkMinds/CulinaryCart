namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public Order Order { get; set; }
    }
}
