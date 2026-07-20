using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryFAL;
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

        public MenuController(MenuDAL menuDal, CategoryDAL categoryDal, DietDAL dietDal, IImageFAL imageFal)
        {
            _menuDal = menuDal;
            _categoryDal = categoryDal;
            _dietDal = dietDal;
            _imageFal = (ImageFAL?)imageFal;
        }


        [HttpGet("ShowMenu")]
        public IActionResult ShowMenu([FromQuery] ShowMenuFilterRequest filter)
        {

            var items = _menuDal.GetAllMenuItems(); 
            
            if (filter.CategoryNames != null && filter.CategoryNames.Any())
            {
                items = items.Where(m => filter.CategoryNames.Contains(m.Category?.CategoryName)).ToList();
            }

            if (filter.DietaryPreferenceNames != null && filter.DietaryPreferenceNames.Any())
            {
                items = items.Where(m => filter.DietaryPreferenceNames.Contains(m.DietaryPreference?.Diet)).ToList();
            }

            //if (!User.IsInRole("Admin"))
            //{
            //    items = items.Where(m => m.InStock).ToList();
            //}

            //pagination
            var totalCount = items.Count;
            var pagedItems = items
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var response = pagedItems.Select(m => new MenuResponse
            {
                FoodItemID = m.FoodItemID,
                FoodItemName = m.FoodItemName,
                Price = m.Price,
                Offers = m.Offers,
                ImageUrl = m.ImageUrl,
                CategoryName = m.Category?.CategoryName,
                DietaryPreferenceName = m.DietaryPreference?.Diet,
                InStock = m.InStock,
                RemainingQuantity = m.RemainingQuantity
            })
                .ToList();

            if (!response.Any())
                return Ok(new { Message = CulinaryCartConstants.Messages.NoMenuItemsAvailable });

            return Ok(new
            {
                TotalFoodItems = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = response
            }
                );
        }

        // Add new menu item
        [HttpPost("AddMenu")]
        [Consumes("multipart/form-data")]
        public IActionResult AddMenu(
            [FromForm] AddMenuRequest request
            )
        {
            var category = _categoryDal.GetAllCategories()
                .FirstOrDefault(c => c.CategoryName == request.CategoryName);
            if (category == null) return BadRequest(CulinaryCartConstants.Messages.InvalidCategoryName);

            var diet = _dietDal.GetAllDietPreferences()
                .FirstOrDefault(d => d.Diet == request.DietaryPreferenceName);
            if (diet == null) return BadRequest(CulinaryCartConstants.Messages.InvalidDietaryPreferenceName);

            var imagePath = _imageFal.SaveImage(request.ImageFile);

            var menuItem = new Menu
            {
                FoodItemName = request.FoodItemName,
                Price = request.Price,
                Offers = request.Offers,
                ImageUrl = imagePath,
                CategoryId = category.CategoryId,
                DietId = diet.DietId,
                RemainingQuantity = 50, // Default quantity
                InStock = true
            };

            var added = _menuDal.AddItem(menuItem);
            return Ok(new { Message = CulinaryCartConstants.Messages.MenuItemAdded, Item = added });
        }

        // Get menu item by ID
        [HttpGet("GetMenu/{id}")]
        public IActionResult GetMenu(int id)
        {
            var menuItem = _menuDal.GetItem(id);
            if (menuItem == null) return NotFound(CulinaryCartConstants.Messages.MenuItemNotFound);

            return Ok(menuItem);
        }
        // Update existing menu item
        [HttpPut("UpdateMenu/{id}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateMenu(
            int id, [FromForm] UpdateMenuRequest request)
        {
            var existing = _menuDal.GetItem(id);
            if (existing == null) return NotFound(CulinaryCartConstants.Messages.MenuItemNotFound);

            if (request.Price.HasValue) existing.Price = request.Price.Value;
            if (!string.IsNullOrEmpty(request.Offers))
                existing.Offers = request.Offers;

            if (request.ImageFile != null)
                existing.ImageUrl = _imageFal.SaveImage(request.ImageFile);

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                var category = _categoryDal.GetAllCategories()
                    .FirstOrDefault(c => c.CategoryName == request.CategoryName);
                if (category == null) return BadRequest(CulinaryCartConstants.Messages.InvalidCategoryName);
                existing.CategoryId = category.CategoryId;
            }

            if (!string.IsNullOrEmpty(request.DietaryPreferenceName))
            {
                var diet = _dietDal.GetAllDietPreferences()
                    .FirstOrDefault(d => d.Diet == request.DietaryPreferenceName);
                if (diet == null) return BadRequest(CulinaryCartConstants.Messages.InvalidDietaryPreferenceName);
                existing.DietId = diet.DietId;
            }

            var success = _menuDal.UpdateItem(id, existing);
            if (!success) return BadRequest(CulinaryCartConstants.Messages.MenuItemUpdateFailed);

            return Ok(new { Message = CulinaryCartConstants.Messages.MenuItemUpdated, Item = existing });
        }
        // Toggle stock status
        [HttpPut("ToggleStock/{id}")]
        public IActionResult ToggleStock(int id, [FromBody] bool inStock)
        {
            var success = _menuDal.ToggleStock(id, inStock);
            if (!success) return BadRequest(CulinaryCartConstants.Messages.StockUpdateFailed);

            
            var updatedItem = _menuDal.GetItem(id);
            if (updatedItem == null) return BadRequest(CulinaryCartConstants.Messages.MenuItemNotFound);

            return Ok(new { Message = CulinaryCartConstants.Messages.StockUpdateSuccessful, Item = updatedItem});
        }

        [HttpPut("Checkout/{id}")]
        public IActionResult Checkout(int id, [FromBody] int quantity)
        {
            var success = _menuDal.CheckoutItems(id, quantity);
            if (!success) return BadRequest("Checkout failed");

            var updatedItem = _menuDal.GetItem(id);
            if (updatedItem == null) return NotFound(CulinaryCartConstants.Messages.MenuItemNotFound);

            return Ok(new { Message = "Stock update successful", Item = updatedItem });
        }
        // Delete menu item
        [HttpDelete("DeleteMenu/{id}")]
        public IActionResult DeleteMenu(int id)
        {
            var deleted = _menuDal.DeleteItem(id);
            if (!deleted)
                return NotFound(new { success = false, message = CulinaryCartConstants.Messages.MenuItemNotFound });

            return Ok(new { success = true, message = CulinaryCartConstants.Messages.MenuItemDeleted });
        }

        [HttpGet("menu-stats")]
        public IActionResult GetMenuStats()
        {
            var items = _menuDal.GetAllMenuItems();

            var totalItems = items.Count;
            var vegItems = items.Count(m => m.DietaryPreference?.Diet == "Veg");
            var nonVegItems = items.Count(m => m.DietaryPreference?.Diet == "Non-Veg");
            var veganItems = items.Count(m => m.DietaryPreference?.Diet == "Vegan");
            var ketoItems = items.Count(m => m.DietaryPreference?.Diet == "Ketogenic");

            var totalCategories = _categoryDal.GetAllCategories().Count();
            var totalDietPreferences = _dietDal.GetAllDietPreferences().Count();

            var categoryDistribution = items
            .GroupBy(m => m.Category?.CategoryName)
            .Select(g => new {CategoryName = g.Key ?? "Uncategorized",
            ItemCount = g.Count()
            }).ToList();

            return Ok(new
            {
                TotalItems = totalItems,
                VegItems = vegItems,
                NonVegItems = nonVegItems,
                VeganItems = veganItems,
                KetoItems = ketoItems,
                TotalCategories = totalCategories,
                TotalDietPreferences = totalDietPreferences,
                CategoryDistribution = categoryDistribution
            });
        }
    }
}