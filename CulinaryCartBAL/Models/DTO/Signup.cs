using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class SignupDto
    {
        [Required, StringLength(100)] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, MinLength(8)] public string Password { get; set; }
    }
}
