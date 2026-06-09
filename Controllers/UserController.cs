using CulinaryCart.CulinaryCartBAL.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserBAL _userBal;

        public AuthController(UserBAL userBal)
        {
            _userBal = userBal;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto Dto) 
        {
            var result = _userBal.Signup(Dto.Name, Dto.Email, Dto.Password); 

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
            var result = _userBal.Login(dto.Email, dto.Password);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { token = result, message = "Login successful" });
        }
    }
}

