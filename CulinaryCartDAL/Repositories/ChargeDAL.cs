using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class ChargeDAL
    {
        private readonly CulinaryCartDbContext _context;

        public ChargeDAL(CulinaryCartDbContext context)
        {
            _context = context;
        }

        // Get all charges
        public List<Charge> GetAllCharges()
        {
            return _context.Charge.ToList();
        }

        // Get charge by ID
        public Charge GetCharge(int id)
        {
            return _context.Charge.FirstOrDefault(c => c.ChargeId == id);
        }

        // Add new charge
        public Charge AddCharge(Charge charge)
        {
            _context.Charge.Add(charge);
            _context.SaveChanges();
            return charge;
        }

        // Update existing charge
        public bool UpdateCharge(int id, Charge updatedCharge)
        {
            var existing = _context.Charge.FirstOrDefault(c => c.ChargeId == id);
            if (existing == null) return false;

            existing.ChargeType = updatedCharge.ChargeType;
            existing.Value = updatedCharge.Value;
            existing.IsActive = updatedCharge.IsActive;

            _context.SaveChanges();
            return true;
        }

        // Delete charge
        public bool DeleteCharge(int id)
        {
            var existing = _context.Charge.FirstOrDefault(c => c.ChargeId == id);
            if (existing == null) return false;

            _context.Charge.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}