using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryFAL;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserBAL _userBal;
        private readonly IImageFAL _imageFal;

        public AuthController(UserBAL userBal, IImageFAL imageFal)
        {
            _userBal = userBal;
            _imageFal = imageFal;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto Dto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userBal.Signup(Dto); 

            if (result == "User already exists")
                return Conflict(new { message = result });

            if (result.StartsWith("Password"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        // Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login dto)
        {
            var result = _userBal.Loginresponse(dto.Email, dto.Password);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });


            return Ok(new { token = result, message = "Login successful" });
        }        
    }
}

