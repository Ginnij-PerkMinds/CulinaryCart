namespace CulinaryCart.CulinaryCartBAL.Models.DTO   
{
    public class MenuRequest
    {
        public string FoodItemName { get; set; }
        public decimal Price { get; set; }
        public string Offers { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string DietaryPreferenceName { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}

