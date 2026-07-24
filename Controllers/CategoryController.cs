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
            var categories = _categoryDal.GetAllCategories();
            return Ok(categories);
        }

        // Add Category
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] CategoryUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { message = "Category Name Required" });

            var existing = _categoryDal.GetByName(request.CategoryName);
            if (existing != null)
                return Conflict(new { message = "Category Already Exists"});

            var category = new Category { CategoryName = request.CategoryName };
            _categoryDal.AddCategory(category);

            return Ok(new { message = "Category Added Successfully"});
        }

        // Get category by ID
        [HttpGet("GetCategory/{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound(CulinaryCartConstants.Messages.CategoryNotFound);

            return Ok(category); 
        }

        // Update category
        [HttpPut("UpdateCategory/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { message = "Category Name Required" });

            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound(new { message = "Category Not Found" });

            var updated = _categoryDal.UpdateCategory(id, request.CategoryName);
            if (!updated)
                return BadRequest(new { message = "Category Update Failed" });

            return Ok(new { message = "Category Updated Successfully"});
        }

        // Delete category
        [HttpDelete("DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var success = _categoryDal.DeleteCategory(id);
            if (!success) return NotFound(new {Message = "Category Not Found"});

            return Ok(new { message = "Category Deleted Successfully"});

        }
    }
}