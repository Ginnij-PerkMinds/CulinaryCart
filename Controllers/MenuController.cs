using CulinaryCart.CulinaryDal;
using CulinaryCart.CulinaryFAL;
using CulinaryCart.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly MenuDAL _menuDal;
        private readonly CategoryDAL _categoryDal;
        private readonly DietDAL _dietDal;
        private readonly ImageFAL _imageFal;

        public MenuController(MenuDAL menuDal, CategoryDAL categoryDal, DietDAL dietDal, ImageFAL imageFal)
        {
            _menuDal = menuDal;
            _categoryDal = categoryDal;
            _dietDal = dietDal;
            _imageFal = imageFal;
        }

        // Show all menu items
        [HttpGet("ShowMenu")]
        public IActionResult ShowMenu()
        {
            var items = _menuDal.GetAllMenuItems()
           .Select(m => new MenuResponse
        {
            FoodItemID = m.FoodItemID,
            FoodItemName = m.FoodItemName,
            Price = m.Price,
            Offers = m.Offers,
            ImageUrl = m.ImageUrl,
            CategoryName = m.Category?.CategoryName,
            DietaryPreferenceName = m.DietaryPreference?.Diet
        })
        .ToList();
            if (!items.Any())
                return Ok(new { Message = "No menu items available" });

            return Ok(items);
        }



        //[HttpPost("AddMenu")]
        //public IActionResult AddMenu([FromForm] MenuRequest request)
        //{
        //    var added = _menuDal.AddItem(menu);
        //    return Ok(new { Message = "Menu item added successfully", Item = added });
        //}

        // Get Categories
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _categoryDal.GetAllCategories();
            return Ok(categories);
        }

        // Get DietaryPreferences
        [HttpGet("GetDietaryPreferences")]
        public IActionResult GetDietaryPreferences()
        {
            var diets = _dietDal.GetAllDietPreferences();
            return Ok(diets);
        }
        // Add new menu item
        [HttpPost("AddMenu")]
        [Consumes("multipart/form-data")]
        public IActionResult AddMenu(
          [FromForm] string foodItemName,
          [FromForm] decimal price,
          [FromForm] string offers,
          IFormFile imageFile,     // for image upload
          //[FromForm] string categoryName,
          //[FromForm] string dietaryPreferenceName)
          [FromForm] int categoryId,
          [FromForm] int dietId)
        {
            var category = _categoryDal.GetById(categoryId);
            if (category == null) 
                return BadRequest("Invalid category name");

            var diet = _dietDal.GetById(dietId);
            if (diet == null) 
                return BadRequest("Invalid dietary preference name");

            var imagePath = _imageFal.SaveImage(imageFile);

            var menuItem = new Menu
            {
                FoodItemName = foodItemName,
                Price = price,
                Offers = offers,
                ImageUrl = imagePath,      // Save image and store path
                CategoryId = category.CategoryId,
                DietId = diet.DietId
            };

            var added = _menuDal.AddItem(menuItem);
            return Ok(new { Message = "Menu item added successfully", Item = added });
        }

        // Update existing menu item
        //public IActionResult UpdateMenu(int id, [FromForm] MenuUpdateRequest request)
        //{
        //    try
        //    {
        //        var updated = _menuDal.UpdateItem(id, request);
        //        if (!updated)
        //            return NotFound("Menu item not found");

        //        return Ok("Menu item updated successfully");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // Update existing menu item
        [HttpPut("UpdateMenu/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateMenu(
            int id,
            [FromForm] decimal? price,
            [FromForm] string? offers,
            IFormFile? imageFile,       // for uploading new image
            //[FromForm] string? categoryName,
            //[FromForm] string? dietaryPreferenceName)
            [FromForm] int categoryId,
            [FromForm] int dietId) 
        {

            var existing = _menuDal.GetItem(id);
            if (existing == null) 
                return NotFound("Menu item not found");

            // Apply updates
            if (price.HasValue)
                existing.Price = price.Value;

            if (!string.IsNullOrEmpty(offers))
                existing.Offers = offers;

            if (imageFile != null)
            {
                // Save new image and update path
                var imagePath = _imageFal.SaveImage(imageFile);
                existing.ImageUrl = imagePath;
            }

            if (categoryId>0)
            {
                var category = _categoryDal.GetById(categoryId);
                if (category == null) 
                    return BadRequest("Invalid categoryId");
                existing.CategoryId = category.CategoryId;
            }

            if (dietId>0)
            {
                var diet = _dietDal.GetById(dietId);
                if (diet == null) 
                    return BadRequest("Invalid dietId");
                existing.DietId = diet.DietId;
            }

            var success = _menuDal.UpdateItem(id, existing);
            if (!success) return BadRequest("Update failed");

            return Ok(new { Message = "Menu item updated successfully", Item = existing });
        }


        // Delete menu item
        [HttpDelete("DeleteMenu/{id}")]
        public IActionResult DeleteMenu(int id)
        {
            var deleted = _menuDal.DeleteItem(id);
            if (!deleted)
                return NotFound("Menu item not found");

            return Ok("Menu item deleted successfully");
        }
    }
 }

