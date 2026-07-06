namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        // Link to User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Collection of CartItems
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
