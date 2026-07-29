using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using System;
using System.Collections.Generic;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class ChargeBAL
    {
        private readonly ChargeDAL _chargeDal;

        public ChargeBAL(ChargeDAL chargeDal)
        {
            _chargeDal = chargeDal;
        }

        // Get all charges
        public IEnumerable<Charge> GetAllCharges()
        {
            return _chargeDal.GetAllCharges();
        }

        // Get charge by ID
        public Charge? GetById(int id)
        {
            if (id <= 0) return null;
            return _chargeDal.GetCharge(id);
        }

        // Add new charge
        public Charge AddCharge(Charge charge)
        {
            if (charge == null) throw new ArgumentNullException(nameof(charge));
            if (string.IsNullOrWhiteSpace(charge.ChargeType))
                throw new ArgumentException("ChargeType cannot be empty.");
            if (charge.Value <= 0)
                throw new ArgumentException("Value must be greater than zero.");

            return _chargeDal.AddCharge(charge);
        }

        // Update charge
        public bool UpdateCharge(Charge updatedCharge)
        {
            if (updatedCharge == null || updatedCharge.ChargeId <= 0)
                return false;
            if (string.IsNullOrWhiteSpace(updatedCharge.ChargeType))
                return false;
            if (updatedCharge.Value <= 0)
                return false;

            return _chargeDal.UpdateCharge(updatedCharge.ChargeId, updatedCharge);
        }

        // Delete charge
        public bool DeleteCharge(int id)
        {
            if (id <= 0) return false;
            return _chargeDal.DeleteCharge(id);
        }
    }
}