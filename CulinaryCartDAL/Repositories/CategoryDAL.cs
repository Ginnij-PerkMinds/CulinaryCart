using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartDAL.Repositories
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

        // Get category by ID
        public Category GetById(int id)
        {
            return _db.Category.FirstOrDefault(c => c.CategoryId == id);
        }

        // Update category
        public bool UpdateCategory(int id, string updatedName)
        {
            var existing = _db.Category.FirstOrDefault(c => c.CategoryId == id);
            if (existing == null) return false;

            existing.CategoryName = updatedName;
            _db.SaveChanges();
            return true;
        }

        // Delete category
        public bool DeleteCategory(int id)
        {
            var existing = _db.Category.FirstOrDefault(c => c.CategoryId == id);
            if (existing == null) return false;

            _db.Category.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}