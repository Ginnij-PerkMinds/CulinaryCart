using CulinaryCart.DbContext;
using CulinaryCart.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryDal
{
    public class CategoryDAL
    {
        private readonly CulinaryCartDbContext _db;

        public CategoryDAL(CulinaryCartDbContext db)
        {
            _db = db;
        }

        // Get all categories
        public List<Category> GetAllCategories()
        {
            return _db.Category.ToList();
        }

        // Get category by ID
        public Category? GetById(int id)
        {
            return _db.Category.FirstOrDefault(c => c.CategoryId == id);
        }

        // Get category by name
        public Category? GetByName(string name)
        {
            return _db.Category.FirstOrDefault(c => c.CategoryName == name);
        }

        // Add new category
        public Category AddCategory(Category category)
        {
            _db.Category.Add(category);
            _db.SaveChanges();
            return category;
        }

        // Update category
        public bool UpdateCategory(int id, Category category)
        {
            var existing = _db.Category.Find(id);
            if (existing == null) return false;

            existing.CategoryName = category.CategoryName;
            _db.SaveChanges();
            return true;
        }

        // Delete category
        public bool DeleteCategory(int id)
        {
            var existing = _db.Category.Find(id);
            if (existing == null) return false;

            _db.Category.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}

