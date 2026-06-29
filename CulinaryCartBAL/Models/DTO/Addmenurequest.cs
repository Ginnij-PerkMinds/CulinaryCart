using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO;

public class AddMenuRequest
{
    [FromForm] public string FoodItemName { get; set; }
    [FromForm] public decimal Price { get; set; }
    [FromForm] public string Offers { get; set; }
    [FromForm] public IFormFile ImageFile { get; set; }
    [FromForm] public string CategoryName { get; set; }
    [FromForm] public string DietaryPreferenceName { get; set; }

}


