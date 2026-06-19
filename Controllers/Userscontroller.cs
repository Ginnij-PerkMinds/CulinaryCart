using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryFAL;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBAL _userBal;
        private readonly IImageFAL _imageFal;

        public UserController(UserBAL userBal, IImageFAL imageFal)
        {
            _userBal = userBal;
            _imageFal = imageFal;
        }

        // Get all users
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userBal.GetAllUsers();
            if (!users.Any())
                return Ok(new { Message = "No users found" });

            return Ok(users);
        }

        // Get user by ID
        [HttpGet("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            var users = _userBal.GetAllUsers();
            var user = users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }

        // Update user
        [HttpPut("UpdateUser/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateUser(int id, [FromForm] UpdateUserDto dto)
        {
            var result = _userBal.UpdateUser(id, dto);

            if (result == "User not found")
                return NotFound(new { Message = result });

            return Ok(new { Message = result });
        }

        // Delete user
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userBal.DeleteUser(id);

            if (result == "User not found")
                return NotFound(new { Message = result });

            return Ok(new { Message = result });
        }
    }
}
