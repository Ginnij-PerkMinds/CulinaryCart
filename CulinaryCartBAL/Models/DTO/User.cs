using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class UpdateFlagsDto
    {
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
        public string? PhoneNo { get; set; }
        public IFormFile? ProfilePic { get; set; }
        

        public string? HouseNo { get; set; }
        public string? Locality { get; set; }
        public string? Landmark { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Pincode { get; set; }
        public string? State { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string ProfilePic { get; set; }
        public string HouseNo { get; set; }
        public string Locality { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public string Address { get; set; }

    }    
}
 



