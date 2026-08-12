using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class RefundsBAL
    {
        private readonly RefundDAL _dal;
        private readonly CulinaryCartDbContext _db;

        public RefundsBAL(RefundDAL dal, CulinaryCartDbContext db)
        {
            _dal = dal;
            _db = db;
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

        //public bool UpdateRefundStatus(int id, string status, string? remarks)
        //{
        //    if (status == "Rejected" && string.IsNullOrWhiteSpace(remarks))
        //        return false;

        //    return _dal.UpdateStatus(id, status, remarks);
        //}
        public bool UpdateRefundStatus(int id, string status, string? remarks, decimal? refundAmount = null)
        {
            var refund = _db.Refunds.FirstOrDefault(r => r.RefundId == id);
            if (refund == null) return false;

            refund.RefundStatus = status;
            refund.Remarks = remarks;

            if (status == "Accepted")
            {
                refund.RefundAmount = refundAmount ?? refund.FinalAmount;
                // use provided amount if partial refund, else full FinalAmount
            }

            _db.Refunds.Update(refund);
            _db.SaveChanges();
            return true;
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
                RefundAmount = r.RefundAmount,
                RefundStatus = r.RefundStatus,
                Remarks = r.Remarks,
                RefundImage = r.RefundImage,
                RefundUserRemarks = r.RefundUserRemarks
            };
        }

        public bool ClaimRefund(int userId, int orderId, int? itemId, string? remarks, string? proofImage)
        {
            var order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
            if (order == null) return false;

            // Eligibility check: last 60 minutes
            if ((DateTime.UtcNow - order.OrderDate).TotalMinutes > 60) return false;

            var refund = new Refund
            {
                OrderId = orderId,
                UserId = userId,
                FinalAmount = order.FinalAmount,
                RefundAmount = 0, // start with 0 untill admin approves
                RefundStatus = "Pending",
                RequestDate = DateTime.UtcNow,
                RefundUserRemarks = remarks,
                RefundImage = proofImage
            };

            _dal.Add(refund);

            // Sync order table for quick status display
            order.RefundStatus = "Pending";
            order.RefundUserRemarks = remarks;
            order.RefundImage = proofImage;
            _db.Orders.Update(order);
            _db.SaveChanges();

            return true;
        }

        public List<RefundDto> GetUserRefunds(int userId)
        {
            return _dal.GetByUser(userId).Select(MapToDto).ToList();
        }

    }
}

