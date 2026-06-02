using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class DietDAL
    {
        private readonly CulinaryCartDbContext _db;

        public DietDAL(CulinaryCartDbContext db)
        {
            _db = db;
        }

        // Get all dietary preferences
        public List<DietaryPreference> GetAllDietPreferences()
        {
            return _db.DietaryPreference.ToList();
        }

        // Get diet by ID
        public DietaryPreference? GetById(int id)
        {
            return _db.DietaryPreference.FirstOrDefault(d => d.DietId == id);
        }

        // Get diet by name
        public DietaryPreference? GetByName(string name)
        {
            return _db.DietaryPreference.FirstOrDefault(d => d.Diet == name);
        }

        // Add new diet preference
        public DietaryPreference AddDietPreference(DietaryPreference diet)
        {
            _db.DietaryPreference.Add(diet);
            _db.SaveChanges();
            return diet;
        }

        // Get dietary preference by ID
        public DietaryPreference GetDietById(int id)
        {
            return _db.DietaryPreference.FirstOrDefault(d => d.DietId == id);
        }

        // Update diet preference
        public bool UpdateDietPreference(int id,string updatedName)
        {
            var existing = _db.DietaryPreference.FirstOrDefault(d => d.DietId == id);
            if (existing == null) return false;

            existing.Diet = updatedName;
            _db.SaveChanges();
            return true;
        }

        // Delete diet preference
        public bool DeleteDietPreference(int id)
        {
            var existing = _db.DietaryPreference.FirstOrDefault(d => d.DietId == id);
            if (existing == null) return false;

            _db.DietaryPreference.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}
