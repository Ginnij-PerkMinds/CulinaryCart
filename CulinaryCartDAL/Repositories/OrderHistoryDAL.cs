using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class OrderHistoryDAL
    {
        private readonly CulinaryCartDbContext _db;

        public OrderHistoryDAL(CulinaryCartDbContext db)
        {
            _db = db;
        }

        // Add a new order with items
        public void Add(Order order)
        {
            //  NEW: ensure breakdown fields are calculated before saving
            CalculateOrderTotals(order);

            _db.Orders.Add(order);
            _db.SaveChanges();
        }

        // Update an existing order (status, totals, etc.)
        public void Update(Order order)
        {
            if (order == null) return;

            // Attach and update the entire entity
            _db.Orders.Update(order);

            // Save changes to persist AppliedPromoCode, PromoDiscount, FinalAmount, etc.
            _db.SaveChanges();

            // Optional: Debug log to confirm persistence
            //Console.WriteLine($"[OrderHistoryDAL.Update] OrderId={order.OrderId}, FinalAmount={order.FinalAmount}, PromoCode={order.AppliedPromoCode}, PromoDiscount={order.PromoDiscount}");

            Console.WriteLine($"[OrderHistoryDAL.Update] " +
                         $"OrderId={order.OrderId}, FinalAmount={order.FinalAmount}, " +
                         $"PromoCode={order.AppliedPromoCode}, PromoDiscount={order.PromoDiscount}, " +
                         $"BaseAmount={order.BaseAmount}, HandlingFee={order.HandlingFee}, " +
                         $"DeliveryFee={order.DeliveryFee}, TaxAmount={order.TaxAmount}");
        }


        // Delete an order (removes order + items)
        public void Delete(Order order)
        {
            _db.Orders.Remove(order);
            _db.SaveChanges();
        }

        // Get all orders with their items
        public List<Order> GetAll()
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Include(o => o.User)
              .ThenInclude(u => u.Address)      // added on 06-08
                      .ToList();
        }

        // Get a single order by ID with items
        public Order? GetById(int orderId)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Include(o => o.User)
              .ThenInclude(u => u.Address)    // added on 06-08
                      .FirstOrDefault(o => o.OrderId == orderId);
        }

        // Get all orders for a specific user
        public List<Order> GetByUser(int userId)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Where(o => o.UserId == userId)
                      .ToList();
        }

        // Get only checked-out orders (useful for stats)
        public List<Order> GetCheckedOutOrders()
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Where(o => o.Status == CulinaryCartConstants.Status.CheckedOut)
                      .ToList();
        }

        // Get all completed orders (CheckedOut or Success)
        public List<Order> GetCompletedOrdersByUser(int userId)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Where(o => o.UserId == userId &&
                                 (o.Status == CulinaryCartConstants.Status.CheckedOut ||
                                  o.Status == CulinaryCartConstants.Status.Success))
                      .OrderByDescending(o => o.OrderDate)
                      .ToList();
        }

        // NEW: Get orders by OrderStatus (Pending, Accepted, Rejected)
        public List<Order> GetOrdersByOrderStatus(string orderStatus)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .Include(o => o.User)
                      .ThenInclude(u => u.Address)
                      .Where(o => o.OrderStatus == orderStatus)
                      .ToList();
        }


        // NEW: Update OrderStatus + Remarks
        public bool UpdateOrderStatus(int orderId, string orderStatus, string? remarks)
        {
            var order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return false;

            order.OrderStatus = orderStatus;
            order.Remarks = remarks;

            _db.Orders.Update(order);
            _db.SaveChanges();

            //Console.WriteLine($"[OrderHistoryDAL.UpdateOrderStatus] OrderId={order.OrderId}, Status={order.OrderStatus}, Remarks={order.Remarks}");
            Console.WriteLine($"[OrderHistoryDAL.UpdateOrderStatus] " +
                              $"OrderId={order.OrderId}, Status={order.OrderStatus}, Remarks={order.Remarks}, " +
                              $"FinalAmount={order.FinalAmount}");
            return true;
        }

        // NEW: helper method to calculate totals consistently
        private void CalculateOrderTotals(Order order)
        {
            if (order == null || !order.OrderItems.Any())
            {
                order.BaseAmount = 0;
                order.PromoDiscount = 0;
                order.HandlingFee = 0;
                order.DeliveryFee = 0;
                order.TaxAmount = 0;
                order.FinalAmount = 0;
                order.TotalAmount = 0;
                return;
            }

            // Base amount = sum of item final prices
            order.BaseAmount = order.OrderItems.Sum(i => i.FinalPrice);

            // Promo discount is set in BAL (Checkout), keep value
            // HandlingFee, DeliveryFee, TaxAmount also set in BAL
            // Here we just ensure FinalAmount and TotalAmount are consistent

            order.FinalAmount = order.BaseAmount - order.PromoDiscount
                                + order.HandlingFee + order.DeliveryFee + order.TaxAmount;

            // Keep legacy TotalAmount in sync
            order.TotalAmount = order.FinalAmount;
        }

        // Save order (alias for Add)
        public void Save(Order order)
        {
            Add(order);
        }

        // Get order by Razorpay OrderId
        public Order GetByRazorpayOrderId(string razorpayOrderId)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
                      .FirstOrDefault(o => o.RazorpayOrderId == razorpayOrderId);
        }
    }
}