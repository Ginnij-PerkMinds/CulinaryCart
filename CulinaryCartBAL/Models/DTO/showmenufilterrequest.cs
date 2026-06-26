using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class ShowMenuFilterRequest
    {
        
        [FromQuery] public string[]? CategoryNames { get; set; }
        [FromQuery] public string[]? DietaryPreferenceNames { get; set; }
        [FromQuery] public int PageNumber { get; set; } = 1;
        [FromQuery] public int PageSize { get; set; } = 10;
    }
}


