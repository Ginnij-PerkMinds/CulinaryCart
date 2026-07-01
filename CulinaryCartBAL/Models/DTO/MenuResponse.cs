using Microsoft.AspNetCore.Mvc;

public class MenuResponse
{
    public int FoodItemID { get; set; }
    public string FoodItemName { get; set; }
    public decimal Price { get; set; }
    public string Offers { get; set; }
    public string ImageUrl { get; set; }
    public string CategoryName { get; set; }
    public string DietaryPreferenceName { get; set; }
    [FromForm] public int CategoryId { get; set; }
    [FromForm] public int DietId { get; set; }
    public bool InStock { get; set; }
}

