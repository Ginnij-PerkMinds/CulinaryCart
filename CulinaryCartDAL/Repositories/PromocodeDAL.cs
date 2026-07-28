using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class PromocodeDAL
    {
        private readonly CulinaryCartDbContext _db;

        public PromocodeDAL(CulinaryCartDbContext db)
        {
            _db = db;
        }

        public List<Promocode> GetAllPromocodes() => _db.Promocode.ToList();

        public Promocode? GetById(int id) => _db.Promocode.FirstOrDefault(p => p.Id == id);

        public Promocode AddPromocode(Promocode promo)
        {
            _db.Promocode.Add(promo);
            _db.SaveChanges();
            return promo;
        }

        public bool UpdatePromocode(int id, Promocode updated)
        {
            var existing = _db.Promocode.FirstOrDefault(p => p.Id == id);
            if (existing == null) return false;

            existing.PromoCodeName = updated.PromoCodeName;
            existing.Amount = updated.Amount;
            existing.Criteria = updated.Criteria;
            existing.FreeDelivery = updated.FreeDelivery;
            existing.UsageCount = updated.UsageCount;
            existing.IsActive = updated.IsActive;

            _db.SaveChanges();
            return true;
        }

        public bool DeletePromocode(int id)
        {
            var existing = _db.Promocode.FirstOrDefault(p => p.Id == id);
            if (existing == null) return false;

            _db.Promocode.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}