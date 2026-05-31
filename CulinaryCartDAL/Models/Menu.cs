using System.ComponentModel.DataAnnotations;

namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Menu
    {
        [Key]
        public int FoodItemID { get; set; }


        public string FoodItemName { get; set; }
        public decimal Price { get; set; }
        public string? Offers { get; set; }
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int DietId { get; set; }
        public DietaryPreference DietaryPreference { get; set; }
    }
}
