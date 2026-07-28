using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromocodeController : ControllerBase
    {
        private readonly PromocodeBAL _bal;

        public PromocodeController(PromocodeBAL bal)
        {
            _bal = bal;
        }

        [HttpGet("GetPromocodes")]
        public IActionResult GetPromocodes() => Ok(_bal.GetAllPromocodes());

        [HttpGet("GetPromocode/{id}")]
        public IActionResult GetPromocode(int id)
        {
            var promo = _bal.GetPromocode(id);
            if (promo == null) return NotFound("Promocode not found");
            return Ok(promo);
        }

        [HttpPost("AddPromocode")]
        public IActionResult AddPromocode([FromBody] Promocode promo)
        {
            if (_bal.AddPromocode(promo))
                return Ok("Promocode added successfully");
            return BadRequest("Failed to add promocode");
        }

        [HttpPut("UpdatePromocode/{id}")]
        public IActionResult UpdatePromocode(int id, [FromBody] Promocode promo)
        {
            if (_bal.UpdatePromocode(id, promo))
                return Ok("Promocode updated successfully");
            return BadRequest("Update failed");
        }

        [HttpDelete("DeletePromocode/{id}")]
        public IActionResult DeletePromocode(int id)
        {
            if (_bal.DeletePromocode(id))
                return Ok("Promocode deleted successfully");
            return NotFound("Promocode not found");
        }
    }

}
