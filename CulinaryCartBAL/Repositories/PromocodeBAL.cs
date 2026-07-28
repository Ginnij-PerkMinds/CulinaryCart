using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class PromocodeBAL
    {
        private readonly PromocodeDAL _dal;

        public PromocodeBAL(PromocodeDAL dal)
        {
            _dal = dal;
        }

        public List<PromocodeDto> GetAllPromocodes()
        {
            return _dal.GetAllPromocodes().Select(p => new PromocodeDto
            {
                Id = p.Id,
                PromoCodeName = p.PromoCodeName,
                Amount = p.Amount,
                Criteria = p.Criteria,
                FreeDelivery = p.FreeDelivery,
                UsageCount = p.UsageCount,
                IsActive = p.IsActive
            }).ToList();
        }

        public PromocodeDto? GetPromocode(int id)
        {
            var promo = _dal.GetById(id);
            if (promo == null) return null;

            return new PromocodeDto
            {
                Id = promo.Id,
                PromoCodeName = promo.PromoCodeName,
                Amount = promo.Amount,
                Criteria = promo.Criteria,
                FreeDelivery = promo.FreeDelivery,
                UsageCount = promo.UsageCount,
                IsActive = promo.IsActive
            };
        }

        public bool AddPromocode(Promocode promo)
        {
            if (promo.Criteria <= 0) return false;
            if (promo.Amount.HasValue && promo.Amount < 0) return false;
            if (promo.UsageCount.HasValue && promo.UsageCount < 0) return false;

            return _dal.AddPromocode(promo) != null;
        }

        public bool UpdatePromocode(int id, Promocode promo)
        {
            if (id <= 0) return false;

            // Validation rules
            if (promo.Criteria <= 0) return false;
            if (promo.Amount.HasValue && promo.Amount < 0) return false;
            if (promo.UsageCount.HasValue && promo.UsageCount < 0) return false;

            return _dal.UpdatePromocode(id, promo);
        }

        public bool DeletePromocode(int id) => _dal.DeletePromocode(id);
    }
}