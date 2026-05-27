namespace CulinaryCart.Model
{
    public class CartItem
    {
        public int CartItemId { get; set; }   // Primary key
        public int FoodItemId { get; set; }   // FK to Menu
        public int Quantity { get; set; }     // Quantity in cart
    }
}

