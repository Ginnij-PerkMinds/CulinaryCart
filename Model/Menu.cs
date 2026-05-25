using System.ComponentModel.DataAnnotations;

namespace CulinaryCart.Model
{
    public class Menu
    {
        [Key]
        public int FoodItemID { get; set; }


        public string FoodItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Offers { get; set; }
        public string ImageUrl { get; set; }
    }
}
