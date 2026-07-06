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

        [HttpPost("AddDietaryPreference")]
        public IActionResult AddDietaryPreference([FromQuery] string dietName)
        {
            if (string.IsNullOrWhiteSpace(dietName))
                return BadRequest(CulinaryCartConstants.Messages.DietaryPreferenceNameRequired);

            var existing = _dietDal.GetByName(dietName);
            if (existing != null)
                return Conflict(CulinaryCartConstants.Messages.AlreadyInDB);

            var diet = new DietaryPreference
            {
                Diet = dietName
            };

            _dietDal.AddDietPreference(diet);
            return Ok(CulinaryCartConstants.Messages.DietaryPreferenceAdded);
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
        [Consumes("multipart/form-data")]
        public IActionResult UpdateDietaryPreference(int id, [FromBody] DietUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Diet))
                return BadRequest(CulinaryCartConstants.Messages.DietaryPreferenceNameRequired);

            var diet = _dietDal.GetById(id);
            if (diet == null)
                return NotFound(CulinaryCartConstants.Messages.DietaryPreferenceNotFound);

            var updated = _dietDal.UpdateDietPreference(id, request.Diet);
            if (!updated)
                return BadRequest(CulinaryCartConstants.Messages.DietaryPreferenceUpdateFailed);

            return Ok(CulinaryCartConstants.Messages.DietaryPreferenceUpdated);
        }


            // Delete dietary preference
            [HttpDelete("DeleteDietaryPreference/{id}")]
            public IActionResult DeleteDietaryPreference(int id)
            {
                var success = _dietDal.DeleteDietPreference(id);
                if (!success) return NotFound(CulinaryCartConstants.Messages.DietaryPreferenceNotFound);

                return Ok(CulinaryCartConstants.Messages.DietaryPreferenceDeleted);
            }
        }
    }


