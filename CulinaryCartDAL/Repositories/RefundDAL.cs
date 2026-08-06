using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class RefundDAL
    {
        private readonly CulinaryCartDbContext _db;

        public RefundDAL(CulinaryCartDbContext db)
        {
            _db = db;
        }

        public List<Refund> GetAll()
        {
            return _db.Refunds
                      .Include(r => r.Order)
                      .Include(r => r.User)
                      .ThenInclude(u => u.Address)
                      .ToList();
        }

        public List<Refund> GetByStatus(string status)
        {
            return _db.Refunds
                      .Include(r => r.Order)
                      .Include(r => r.User)
                      .Where(r => r.RefundStatus == status)
                      .ToList();
        }

        public Refund? GetById(int id)
        {
            return _db.Refunds
                      .Include(r => r.Order)
                      .ThenInclude(o => o.OrderItems)        // load order items
                      .Include(r => r.User)
                      .ThenInclude(u => u.Address)
                      .FirstOrDefault(r => r.RefundId == id);
        }

        public bool UpdateStatus(int id, string status, string? remarks)
        {
            var refund = _db.Refunds.FirstOrDefault(r => r.RefundId == id);
            if (refund == null) return false;

            refund.RefundStatus = status;
            refund.Remarks = remarks;

            _db.Refunds.Update(refund);
            _db.SaveChanges();
            return true;
        }
    }
}

