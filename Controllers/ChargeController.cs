using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargeController : ControllerBase
    {
        private readonly ChargeBAL _chargeBal;

        public ChargeController(ChargeBAL chargeBal)
        {
            _chargeBal = chargeBal;
        }

        // GET: api/charge
        [HttpGet]
        public IActionResult GetAllCharges()
        {
            var charges = _chargeBal.GetAllCharges();
            return Ok(charges.Select(c => new ChargeDto
            {
                ChargeId = c.ChargeId,
                ChargeType = c.ChargeType,
                Value = c.Value,
                IsActive = c.IsActive
            }));
        }

        // GET: api/charge/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetChargeById(int id)
        {
            var charge = _chargeBal.GetById(id);
            if (charge == null) return NotFound();

            return Ok(new ChargeDto
            {
                ChargeId = charge.ChargeId,
                ChargeType = charge.ChargeType,
                Value = charge.Value,
                IsActive = charge.IsActive
            });
        }

        // Add
        [HttpPost]
        public IActionResult AddCharge([FromBody] AddChargeRequest request)
        {
            if (request == null) return BadRequest("Request cannot be null.");
            if (string.IsNullOrWhiteSpace(request.ChargeType))
                return BadRequest("ChargeType cannot be empty.");
            if (request.Value <= 0)
                return BadRequest("Value must be greater than zero.");

            var created = _chargeBal.AddCharge(new Charge
            {
                ChargeType = request.ChargeType,
                Value = request.Value,   // ✅ already decimal
                IsActive = request.IsActive
            });
            return Ok(new { message = "Charge Saved successfully" });
        }

        // Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateCharge(int id, [FromBody] UpdateChargeRequest request)
        {
            if (request == null) return BadRequest("Request cannot be null.");
            if (id != request.ChargeId) return BadRequest("ID mismatch.");
            if (request.Value <= 0) return BadRequest("Value must be greater than zero.");

            var success = _chargeBal.UpdateCharge(new Charge
            {
                ChargeId = request.ChargeId,
                ChargeType = request.ChargeType,
                Value = request.Value,   // ✅ already decimal
                IsActive = request.IsActive
            });

            if (!success) return NotFound();
            return Ok(new { message = "Charge updated successfully" });
        }


        // DELETE
        [HttpDelete("{id:int}")]
        public IActionResult DeleteCharge(int id)
        {
            var success = _chargeBal.DeleteCharge(id);
            if (!success) return NotFound();
            return Ok(new { message = "Charge deleted successfully" });
        }
    }
}