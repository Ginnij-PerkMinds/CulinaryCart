public class MenuUpdateRequest
{
    public string FoodItemName { get; set; }          // identify which item to update
    public decimal? Price { get; set; }               // optional new price
    public string Offers { get; set; }                // optional new offers
    public string ImageUrl { get; set; }              // optional new image
    public string CategoryName { get; set; }          // optional new category
    public string DietaryPreferenceName { get; set; } // optional new diet preference
}

