using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO;

public class ShowMenuFilterRequest
{
    [FromQuery] public string? CategoryName { get; set; }
    [FromQuery] public string? DietaryPreferenceName { get; set; }

    [FromQuery] public int PageNumber { get; set; } = 1;   
    [FromQuery] public int PageSize { get; set; } = 10;
}

