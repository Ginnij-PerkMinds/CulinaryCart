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
                RefundAmount = refund.RefundAmount,
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
                }).ToList(),

                // ✅ New: include refund items
                RefundItems = refund.RefundItems.Select(ri => new RefundItemDto
                {
                    RefundItemId = ri.RefundItemId,
                    FoodItemId = ri.FoodItemID,
                    FoodItemName = ri.Menu.FoodItemName,
                    RefundImage = ri.RefundImage,
                    Remarks = ri.Remarks
                }).ToList()
            };
        }
        

        public bool UpdateRefundStatus(int id, string status, string? remarks, decimal? refundAmount = null)
        {
            var refund = _db.Refunds.FirstOrDefault(r => r.RefundId == id);
            if (refund == null) return false;

            refund.RefundStatus = status;
            refund.Remarks = remarks;

            if (status == "Accepted")
            {
                refund.RefundAmount = refund.RefundAmount;
                
                var order = _db.Orders.FirstOrDefault(o => o.OrderId == refund.OrderId);
                if (order != null)
                {
                    order.OrderStatus = "Refunded";
                    _db.Orders.Update(order);
                }
            }
            else if (status == "Rejected")
            {
                refund.RefundAmount = 0;   // reset to 0
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
        public bool ClaimRefund(int userId, int orderId, List<RefundItemRequestDto>? items, decimal refundAmount)
        {
            var order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
            if (order == null) return false;

            var existingRefund = _db.Refunds.FirstOrDefault(r => r.OrderId == orderId && r.UserId == userId);
            if (existingRefund != null) return false;

            if ((DateTime.UtcNow - order.OrderDate).TotalMinutes < 120) return false;

            var refund = new Refund
            {
                OrderId = orderId,
                UserId = userId,
                FinalAmount = order.FinalAmount,
                RefundAmount = refundAmount,
                RefundStatus = "Pending",
                RequestDate = DateTime.UtcNow,
                RefundItems = new List<RefundItem>()
            };

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    refund.RefundItems.Add(new RefundItem
                    {
                        FoodItemID = item.FoodItemId,   // ✅ correct property
                        RefundImage = item.ProofImage,  // ✅ string path
                        Remarks = item.Remarks          // ✅ correct casing
                    });
                }
            }

            _dal.Add(refund);

            order.RefundStatus = "Pending";
            _db.Orders.Update(order);
            _db.SaveChanges();

            return true;
        }

        //public bool ClaimRefund(int userId, int orderId, List<RefundItemRequestDto>? items, decimal refundAmount)
        //{

        //    var order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        //    if (order == null) return false;

        //    // Prevent duplicate refund claims
        //    var existingRefund = _db.Refunds.FirstOrDefault(r => r.OrderId == orderId && r.UserId == userId);
        //    if (existingRefund != null)
        //    {
        //        // Already claimed
        //        return false; // or throw a custom exception / return a message
        //    }

        //    // Eligibility check: last 120 minutes
        //    if ((DateTime.UtcNow - order.OrderDate).TotalMinutes < 120) return false;

        //    var refund = new Refund
        //    {
        //        OrderId = orderId,
        //        UserId = userId,
        //        FinalAmount = order.FinalAmount,
        //        RefundAmount = refundAmount,
        //        RefundStatus = "Pending",
        //        RequestDate = DateTime.UtcNow,
        //        //RefundUserRemarks = remarks,
        //        //RefundImage = proofImage,
        //        RefundItems = new List<RefundItem>()
        //    };
        //        if (items != null && items.Any())
        //        {
        //        foreach (var itemId in items)
        //        {
        //            refund.RefundItems.Add(new RefundItem
        //            {
        //                FoodItemID = item.FoodItemID,
        //                RefundImage = item.ProofImage,
        //                Remarks = item.remarks
        //            });
        //        }
        //        }


        //    _dal.Add(refund);

        //    // Sync order table for quick status display
        //    order.RefundStatus = "Pending";
        //    //order.RefundUserRemarks = remarks;
        //    //order.RefundImage = proofImage;
        //    _db.Orders.Update(order);
        //    _db.SaveChanges();

        //    return true;
        //}

        public List<RefundDto> GetUserRefunds(int userId)
        {
            return _dal.GetByUser(userId).Select(MapToDto).ToList();
        }

    }
}

