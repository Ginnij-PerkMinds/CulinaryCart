using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.AspNetCore.Mvc;
using CulinaryCart.CulinaryCartBAL.Models.DTO;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DietaryPreferenceController : ControllerBase
    {
        private readonly DietDAL _dietDal;

        public DietaryPreferenceController(DietDAL dietDal)
        {
            _dietDal = dietDal;
        }

        [HttpGet("GetDietaryPreferences")]
        public IActionResult GetDietaryPreferences()
        {
            var diets = _dietDal.GetAllDietPreferences();
            return Ok(diets);
        }

        [HttpPost("AddDietaryPreference")]
        public IActionResult AddDietaryPreference([FromQuery] string dietName)
        {
            if (string.IsNullOrWhiteSpace(dietName))
                return BadRequest("Dietary preference name is required.");

            var existing = _dietDal.GetByName(dietName);
            if (existing != null)
                return Conflict("Already in DB");

            var diet = new DietaryPreference
            {
                Diet = dietName
            };

            _dietDal.AddDietPreference(diet);
            return Ok("Dietary preference added successfully.");
        }

        // Get dietary preference by ID
        [HttpGet("GetDietaryPreference/{id}")]
        public IActionResult GetDietaryPreference(int id)
        {
            var diet = _dietDal.GetById(id);
            if (diet == null) return NotFound("Dietary preference not found");

            return Ok(diet);
        }

        // Update dietary preference
        [HttpPut("UpdateDietaryPreference/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateDietaryPreference(int id, [FromBody] DietUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Diet))
                return BadRequest("New dietary preference name is required.");

            var diet = _dietDal.GetById(id);
            if (diet == null)
                return NotFound("Dietary preference not found");

            var updated = _dietDal.UpdateDietPreference(id, request.Diet);
            if (!updated)
                return BadRequest("Update failed");

            return Ok("Dietary preference updated successfully.");
        }


            // Delete dietary preference
            [HttpDelete("DeleteDietaryPreference/{id}")]
            public IActionResult DeleteDietaryPreference(int id)
            {
                var success = _dietDal.DeleteDietPreference(id);
                if (!success) return NotFound("Dietary preference not found");

                return Ok("Dietary preference deleted successfully.");
            }
        }
    }


