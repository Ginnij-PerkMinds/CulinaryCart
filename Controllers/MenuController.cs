using CulinaryCart.CulinaryDal;
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

        public MenuController(MenuDAL menuDal, CategoryDAL categoryDal, DietDAL dietDal)
        {
            _menuDal = menuDal;
            _categoryDal = categoryDal;
            _dietDal = dietDal;
        }

        // ✅ Show all menu items
        [HttpGet("ShowMenu")]
        public IActionResult ShowMenu()
        {
            //var items = _menuDal.GetAllMenuItems();
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


        // ✅ Add new menu item
        //[HttpPost("AddMenu")]
        //public IActionResult AddMenu([FromBody] Menu menu)
        //{
        //    var added = _menuDal.AddItem(menu);
        //    return Ok(new { Message = "Menu item added successfully", Item = added });
        //}
        [HttpPost("AddMenu")]
        public IActionResult AddMenu(
          string foodItemName,
          decimal price,
          string offers,
          string imageUrl,
          string categoryName,
          string dietaryPreferenceName)
        {
            var category = _categoryDal.GetByName(categoryName);
            if (category == null) return BadRequest("Invalid category name");

            var diet = _dietDal.GetByName(dietaryPreferenceName);
            if (diet == null) return BadRequest("Invalid dietary preference name");

            var menuItem = new Menu
            {
                FoodItemName = foodItemName,
                Price = price,
                Offers = offers,
                ImageUrl = imageUrl,
                CategoryId = category.CategoryId,
                DietId = diet.DietId
            };

            var added = _menuDal.AddItem(menuItem);
            return Ok(new { Message = "Menu item added successfully", Item = added });
        }

        // ✅ Update existing menu item
        [HttpPut("UpdateMenu/{id}")]
        public IActionResult UpdateMenu(int id, [FromBody] Menu menu)
        {
            var updated = _menuDal.UpdateItem(id, menu);
            if (!updated)
                return NotFound(new { Message = "Menu item not found" });

            return Ok(new { Message = "Menu item updated successfully" });
        }

        // ✅ Delete menu item
        [HttpDelete("DeleteMenu/{id}")]
        public IActionResult DeleteMenu(int id)
        {
            var deleted = _menuDal.DeleteItem(id);
            if (!deleted)
                return NotFound(new { Message = "Menu item not found" });

            return Ok(new { Message = "Menu item deleted successfully" });
        }
    }
 }

