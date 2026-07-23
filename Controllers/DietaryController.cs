using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.AspNetCore.Mvc;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Constants;

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
        
        // Add Dietary Preference
        [HttpPost("AddDietaryPreference")]
        public IActionResult AddDietaryPreference([FromBody] DietUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Diet))
                return BadRequest(new { message = "Dietary Preference name required"});

            var existing = _dietDal.GetByName(request.Diet);
            if (existing != null)
                return Conflict(new {message = "Already in Database"});

            var diet = new DietaryPreference { Diet = request.Diet };
            _dietDal.AddDietPreference(diet);

            return Ok(new { message = "Dietary preference added successfully!" });
        }

        // Get dietary preference by ID
        [HttpGet("GetDietaryPreference/{id}")]
        public IActionResult GetDietaryPreference(int id)
        {
            var diet = _dietDal.GetById(id);
            if (diet == null) return NotFound(CulinaryCartConstants.Messages.DietaryPreferenceNotFound);

            return Ok(diet);
        }

        // Update dietary preference
        [HttpPut("UpdateDietaryPreference/{id}")]
        public IActionResult UpdateDietaryPreference(int id, [FromBody] DietUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Diet))
                return BadRequest(new { message = "Dietary Preference Name Required" });

            var diet = _dietDal.GetById(id);
            if (diet == null)
                return NotFound(new { message = "Dietary Preference Not Found" });

            var updated = _dietDal.UpdateDietPreference(id, request.Diet);
            if (!updated)
                return BadRequest(new { message = "Dietary Preference Update Failed" });

            return Ok(new { message = "Dietary Preference Successfully Updated"});
        }

        // Delete dietary preference
        [HttpDelete("DeleteDietaryPreference/{id}")]
        public IActionResult DeleteDietaryPreference(int id)
        {
            var success = _dietDal.DeleteDietPreference(id);
            if (!success) return NotFound(new {Message = "Dietary Preference Not Found"});

            return Ok(new { messages = "DietaryPreference Successfully Deleted"});
        }
    }
}