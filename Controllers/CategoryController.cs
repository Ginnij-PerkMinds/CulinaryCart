using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Constants;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryDAL _categoryDal;

        public CategoryController(CategoryDAL categoryDal)
        {
            _categoryDal = categoryDal;
        }

        // Show all categories
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _categoryDal.GetAllCategories()
            .Select(c => new CategoryDto
             {
                 Id = c.CategoryId,
                 Name = c.CategoryName
             }).ToList();
            return Ok(categories);
        }

        // Add new category
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(CulinaryCartConstants.Messages.CategoryNameRequired);

            var existing = _categoryDal.GetByName(dto.Name);
            if (existing != null)
                return Conflict(CulinaryCartConstants.Messages.AlreadyInDB);

            var category = new Category { CategoryName = dto.Name };
            _categoryDal.AddCategory(category);

            return Ok(new CategoryDto { Id = category.CategoryId, Name = category.CategoryName });
        }

        // Get category by ID
        [HttpGet("GetCategory/{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound(CulinaryCartConstants.Messages.CategoryNotFound);
            var dto = new CategoryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName
            };
            return Ok(category); 
        }

        // Update category
        [HttpPut("UpdateCategory/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
        {
            
            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(CulinaryCartConstants.Messages.CategoryUpdateNameRequired);

            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound(CulinaryCartConstants.Messages.CategoryNotFound);

            var updated = _categoryDal.UpdateCategory(id, request.CategoryName);
            if (!updated)
                return BadRequest(CulinaryCartConstants.Messages.CategoryUpdateFailed);

            return Ok(CulinaryCartConstants.Messages.CategoryUpdated);
        }

        // Delete category
        [HttpDelete("DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var success = _categoryDal.DeleteCategory(id);
            if (!success) return NotFound(CulinaryCartConstants.Messages.CategoryNotFound);

            return Ok(CulinaryCartConstants.Messages.CategoryDeleted);

        }
    }
}
