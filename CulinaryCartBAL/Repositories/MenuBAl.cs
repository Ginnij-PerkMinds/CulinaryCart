using System.Collections.Generic;
using CulinaryCart.CulinaryFAL;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartBAL.Constants;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class MenuBAL
    {
        private readonly MenuDAL _menuDal;
        private readonly CategoryDAL _categoryDal;
        private readonly DietDAL _dietDal;
        private readonly IImageFAL _imageFal;  //image  upload dependency

        public MenuBAL(MenuDAL menuDal, CategoryDAL categoryDal, DietDAL dietDal, IImageFAL imageFal)
        {
            _menuDal = menuDal;
            _categoryDal = categoryDal;
            _dietDal = dietDal;
            _imageFal = imageFal; // injected via constructor
        }

        // Show all menu items
        public IEnumerable<Menu> ShowMenu()
        {
            return _menuDal.GetAllMenuItems();
        }

        // Get single menu item by ID
        public Menu? GetMenuItem(int id)
        {
            return _menuDal.GetItem(id);
        }

        // Add new menu item 
        public Menu AddMenu(MenuRequest request)
        {
            var category = _categoryDal.GetByName(request.CategoryName);
            if (category == null) 
                throw new System.Exception(CulinaryCartConstants.Messages.InvalidCategoryName);

            var diet = _dietDal.GetByName(request.DietaryPreferenceName);
            if (diet == null) 
                throw new System.Exception(CulinaryCartConstants.Messages.InvalidDietaryPreferenceName);

            //save image and get path
            var imagePath = _imageFal.SaveImage(request.ImageFile);

            var menu = new Menu
            {
                FoodItemName = request.FoodItemName,
                Price = request.Price,
                Offers = request.Offers,
                ImageUrl = imagePath,
                CategoryId = category.CategoryId,
                DietId = diet.DietId,
                RemainingQuantity=50
            };

            return _menuDal.AddItem(menu);
        }

        // Update existing menu item
        public bool UpdateMenu(int id, MenuUpdateRequest request)
        {
            var existing = _menuDal.GetItem(id);
            if (existing == null) return false;

            if (request.Price.HasValue)
                existing.Price = request.Price.Value;

            if (!string.IsNullOrEmpty(request.Offers))
                existing.Offers = request.Offers;

            //if (!string.IsNullOrEmpty(request.ImageUrl))
            //    existing.ImageUrl = request.ImageUrl;
            if (request.ImageFile != null)
            {
                var imagePath = _imageFal.SaveImage(request.ImageFile);
                existing.ImageUrl = imagePath;
            }   

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                var category = _categoryDal.GetByName(request.CategoryName);
                if (category == null)
                    throw new Exception(CulinaryCartConstants.Messages.InvalidCategoryName);
                existing.CategoryId = category.CategoryId;
            }

            if (!string.IsNullOrEmpty(request.DietaryPreferenceName))
            {
                var diet = _dietDal.GetByName(request.DietaryPreferenceName);
                if (diet == null) 
                    throw new Exception(CulinaryCartConstants.Messages.InvalidDietaryPreferenceName);
                existing.DietId = diet.DietId;
            }

            return _menuDal.UpdateItem(id, existing);
        }

        // Delete menu item
        public bool DeleteMenu(int id)
        {
            var existing = _menuDal.GetItem(id);
            if (existing == null) return false;

            if (!string.IsNullOrEmpty(existing.ImageUrl))
            {
                _imageFal.DeleteImage(existing.ImageUrl);
            }

            return _menuDal.DeleteItem(id);
            
        }

    }
}
