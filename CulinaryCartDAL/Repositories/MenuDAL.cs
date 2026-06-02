using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class MenuDAL
    {
        private readonly CulinaryCartDbContext _db;
        public MenuDAL(CulinaryCartDbContext db) 
        { 
            _db = db; 
        }

        // Get all menu items
        public List<Menu> GetAllMenuItems()
        {
            return _db.Menu
                .Include(m => m.Category)
                .Include(m => m.DietaryPreference)
                .ToList();
        }

        // Get menu item by ID
        public Menu? GetItem(int id)
        {
            return _db.Menu
                .Include(m => m.Category)
                .Include(m => m.DietaryPreference)
                .FirstOrDefault(m => m.FoodItemID == id);
        }

        // Add new item
        public Menu AddItem(Menu menu)
        {
            _db.Menu.Add(menu);
            _db.SaveChanges();
            return menu;
        }

        // Update existing item
        public bool UpdateItem(int id, Menu menu)
        {
            var existing = _db.Menu.Find(id);
            if (existing == null) return false;

            existing.FoodItemName = menu.FoodItemName;
            existing.Price = menu.Price;
            existing.Offers = menu.Offers;
            existing.ImageUrl = menu.ImageUrl;
            existing.CategoryId = menu.CategoryId;
            existing.DietId = menu.DietId;

            _db.SaveChanges();
            return true;
        }

        // Delete item
        public bool DeleteItem(int id)
        {
            var existing = _db.Menu.Find(id);
            if (existing == null) return false;

            _db.Menu.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        // Filter + Pagination
        public List<Menu> GetFilteredMenu(int? categoryId, int? dietId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _db.Menu
                .Include(m => m.Category)
                .Include(m => m.DietaryPreference)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(m => m.CategoryId == categoryId.Value);

            if (dietId.HasValue)
                query = query.Where(m => m.DietId == dietId.Value);

            return query
                .OrderBy(m => m.FoodItemID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // Count for pagination metadata
        public int CountFilteredMenu(int? categoryId, int? dietId)
        {
            var query = _db.Menu.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(m => m.CategoryId == categoryId.Value);

            if (dietId.HasValue)
                query = query.Where(m => m.DietId == dietId.Value);

            return query.Count();
        }

    }
}
