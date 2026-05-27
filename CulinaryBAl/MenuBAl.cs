using CulinaryCart.CulinaryDal;
using CulinaryCart.Model;
using System.Collections.Generic;

namespace CulinaryCart.CulinaryBAL
{
    public class MenuBAL
    {
        private readonly MenuDAL _menuDal;
        private readonly CategoryDAL _categoryDal;
        private readonly DietDAL _dietDal;

        public MenuBAL(MenuDAL menuDal, CategoryDAL categoryDal, DietDAL dietDal)
        {
            _menuDal = menuDal;
            _categoryDal = categoryDal;
            _dietDal = dietDal;
        }

        // ✅ Show all menu items
        public IEnumerable<Menu> ShowMenu()
        {
            return _menuDal.GetAllMenuItems();
        }

        // ✅ Get single menu item by ID
        public Menu? GetMenuItem(int id)
        {
            return _menuDal.GetItem(id);
        }

        // ✅ Add new menu item (maps names → IDs)
        public Menu AddMenu(MenuRequest request)
        {
            var category = _categoryDal.GetByName(request.CategoryName);
            if (category == null) throw new System.Exception("Invalid category name");

            var diet = _dietDal.GetByName(request.DietaryPreferenceName);
            if (diet == null) throw new System.Exception("Invalid dietary preference name");

            var menu = new Menu
            {
                FoodItemName = request.FoodItemName,
                Price = request.Price,
                Offers = request.Offers,
                ImageUrl = request.ImageUrl,
                CategoryId = category.CategoryId,
                DietId = diet.DietId
            };

            return _menuDal.AddItem(menu);
        }

        // ✅ Update existing menu item
        public bool UpdateMenu(int id, MenuUpdateRequest request)
        {
            var existing = _menuDal.GetItem(id);
            if (existing == null) return false;

            if (request.Price.HasValue)
                existing.Price = request.Price.Value;

            if (!string.IsNullOrEmpty(request.Offers))
                existing.Offers = request.Offers;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                existing.ImageUrl = request.ImageUrl;

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                var category = _categoryDal.GetByName(request.CategoryName);
                if (category == null) throw new System.Exception("Invalid category name");
                existing.CategoryId = category.CategoryId;
            }

            if (!string.IsNullOrEmpty(request.DietaryPreferenceName))
            {
                var diet = _dietDal.GetByName(request.DietaryPreferenceName);
                if (diet == null) throw new System.Exception("Invalid dietary preference name");
                existing.DietId = diet.DietId;
            }

            return _menuDal.UpdateItem(id, existing);
        }

        // ✅ Delete menu item
        public bool DeleteMenu(int id)
        {
            return _menuDal.DeleteItem(id);
        }
    }
}
