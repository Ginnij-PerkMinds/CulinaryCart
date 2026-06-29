using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryFAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBAL _userBal;

        public UserController(UserBAL userBal, IImageFAL imageFal)
        {
            _userBal = userBal;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userBal.GetAllUsers();
            return Ok(users);
        }
        [Authorize]
        [HttpGet("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            var users = _userBal.GetAllUsers();
            var user = users.FirstOrDefault(u => u.UserId == id);
            return user == null ? NotFound(new { Message = "User not found" }) : Ok(user);
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userIdClaim = HttpContext.User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var users = _userBal.GetAllUsers();
            var user = users.FirstOrDefault(u => u.UserId == userId);

            return user == null
                ? NotFound(new { Message = "User not found" })
                : Ok(user);
        }

        // Unified update endpoint
        // For toggles (JSON)
        [HttpPut("UpdateFlags/{id}")]
        public IActionResult UpdateFlags(int id, [FromBody] UpdateFlagsDto dto)
        {
            var result = _userBal.UpdateFlags(id, dto);
            return result == "User not found"
                ? NotFound(new { Message = result })
                : Ok(new { Message = result });
        }

        // For profile updates (FormData)
        [HttpPut("UpdateUserForm/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateUserProfile(int id, [FromForm] UpdateUserDto dto)
        {
            var result = _userBal.UpdateUserProfile(id, dto);
            if (result.Contains("exists")) return Conflict(new { Message = result });    // <-- added 
            return result == "User not found"
                ? NotFound(new { Message = result })
                : Ok(new { Message = result });
        }

        // For password change (JSON)    <-- added
        [HttpPut("ChangePassword/{id}")]
        public IActionResult ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var result = _userBal.ChangePassword(id, dto.OldPassword, dto.NewPassword);
            if (result == "User not found") return NotFound(new { Message = result });
            if (result == "Incorrect old password") return BadRequest(new { Message = result });
            if (result.StartsWith("Password must")) return BadRequest(new { Message = result });
            return Ok(new { Message = result });
        }

        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userBal.DeleteUser(id);
            return result == "User not found" ? NotFound(new { Message = result }) : Ok(new { Message = result });
        }
    }
}




