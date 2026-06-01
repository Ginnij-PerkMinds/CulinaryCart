using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO;

public class ShowMenuFilterRequest
{
    [FromQuery] public string? CategoryName { get; set; }
    [FromQuery] public string? DietaryPreferenceName { get; set; }
}

