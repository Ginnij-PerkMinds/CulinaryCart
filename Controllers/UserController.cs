using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryFAL;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using static CulinaryCart.CulinaryCartBAL.Constants.CulinaryCartConstants;

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

            if (result == CulinaryCartConstants.Messages.UserAlreadyExists)
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
                return Unauthorized(new { message = CulinaryCartConstants.Messages.InvalidEmailOrPassword });


            return Ok(new
            {
                token = result.Token,                    
                message = CulinaryCartConstants.Messages.LoginSuccessful,
                user = new
                {
                    userId = result.UserId,
                    name = result.Name,
                    emailId = result.Email,
                    phoneNo = result.PhoneNo,
                    profilePic = result.ProfilePic,
                    isAdmin = result.IsAdmin
                }
            });
        }

        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var result = _userBal.Logout(request.Token);
            if (result == CulinaryCartConstants.Messages.LogoutSuccessful)
                return Ok(new { Message = result });

            return BadRequest(result);
        }

    }
}

