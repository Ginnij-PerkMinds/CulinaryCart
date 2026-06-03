using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBAL _userBal;

        public UserController(UserBAL userBal)
        {
            _userBal = userBal;
        }

        [HttpPost("signup")]
        public IActionResult Signup(string name, string email, string password)
        {
            var result = _userBal.Signup(name, email, password);

            if (result == "User already exists")
                return Conflict(result);

            if (result.StartsWith("Password"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}

