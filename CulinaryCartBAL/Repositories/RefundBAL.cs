using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class RefundsBAL
    {
        private readonly RefundDAL _dal;

        public RefundsBAL(RefundDAL dal)
        {
            _dal = dal;
        }

        public List<RefundDto> GetAllRefunds()
        {
            return _dal.GetAll().Select(MapToDto).ToList();
        }

        public List<RefundDto> GetRefundsByStatus(string status)
        {
            return _dal.GetByStatus(status).Select(MapToDto).ToList();
        }

        public RefundDetailsDto? GetRefundDetails(int id)
        {
            var refund = _dal.GetById(id);
            if (refund == null) return null;

            return new RefundDetailsDto
            {
                RefundId = refund.RefundId,
                RequestDate = refund.RequestDate,
                Username = refund.User?.Name,
                Address = $"{refund.User?.Address?.HouseNo}, {refund.User?.Address?.City}",
                PhoneNo = refund.User?.PhoneNo,
                FinalAmount = refund.FinalAmount,
                RefundStatus = refund.RefundStatus,
                Remarks = refund.Remarks,
                OrderId = refund.OrderId,
                BaseAmount = refund.Order.BaseAmount,
                PromoDiscount = refund.Order.PromoDiscount,
                HandlingFee = refund.Order.HandlingFee,
                DeliveryFee = refund.Order.DeliveryFee,
                TaxAmount = refund.Order.TaxAmount,

                OrderItems = refund.Order.OrderItems.Select(oi => new OrderItemDto
                {
                    FoodItemId = oi.FoodItemId,
                    FoodItemName = oi.FoodItemName,   // assumes you have Item navigation property
                    Quantity = oi.Quantity,
                    FinalPrice = oi.FinalPrice
                }).ToList()
            };
        }

        public bool UpdateRefundStatus(int id, string status, string? remarks)
        {
            if (status == "Rejected" && string.IsNullOrWhiteSpace(remarks))
                return false;

            return _dal.UpdateStatus(id, status, remarks);
        }

        private RefundDto MapToDto(Refund r)
        {
            return new RefundDto
            {
                RefundId = r.RefundId,
                RequestDate = r.RequestDate,
                Username = r.User?.Name,
                Address = $"{r.User?.Address?.HouseNo}, {r.User?.Address?.City}",
                PhoneNo = r.User?.PhoneNo,
                FinalAmount = r.FinalAmount,
                RefundStatus = r.RefundStatus,
                Remarks = r.Remarks
            };
        }
    }
}

