//using CulinaryCart.CulinaryCartBAL.Constants;
//using CulinaryCart.CulinaryCartDAL.DbContext;
//using CulinaryCart.CulinaryCartDAL.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;

//namespace CulinaryCart.CulinaryCartDAL.Repositories
//{
//    public class OrderHistoryDAL
//    {
//        private readonly CulinaryCartDbContext _db;

//        public OrderHistoryDAL(CulinaryCartDbContext db)
//        {
//            _db = db;
//        }

//        // Add a new order with items
//        public void Add(Order order)
//        {
//            _db.Orders.Add(order);
//            _db.SaveChanges();
//        }

//        // Update an existing order (status, totals, etc.)
//        public void Update(Order order)
//        {
//            _db.Orders.Update(order);
//            _db.SaveChanges();
//        }

//        // Delete an order (removes order + items)
//        public void Delete(Order order)
//        {
//            _db.Orders.Remove(order);
//            _db.SaveChanges();
//        }

//        // Get all orders with their items
//        public List<Order> GetAll()
//        {
//            return _db.Orders
//                      .Include(o => o.OrderItems)
//                      .ToList();
//        }

//        // Get a single order by ID with items
//        public Order? GetById(int orderId)
//        {
//            return _db.Orders
//                      .Include(o => o.OrderItems)
//                      .FirstOrDefault(o => o.OrderId == orderId);
//        }

//        // Get all orders for a specific user
//        public List<Order> GetByUser(int userId)
//        {
//            return _db.Orders
//                      .Include(o => o.OrderItems)
//                      .Where(o => o.UserId == userId)
//                      .ToList();
//        }

//        // Get only checked-out orders (useful for stats)
//        public List<Order> GetCheckedOutOrders()
//        {
//            return _db.Orders
//                      .Include(o => o.OrderItems)
//                      .Where(o => o.Status == CulinaryCartConstants.Status.CheckedOut)
//                      .ToList();
//        }
//    }
//}

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
            // 🔹 NEW: ensure breakdown fields are calculated before saving
            CalculateOrderTotals(order);

            _db.Orders.Add(order);
            _db.SaveChanges();
        }

        // Update an existing order (status, totals, etc.)
        public void Update(Order order)
        {
            // 🔹 NEW: recalc totals whenever updating
            CalculateOrderTotals(order);

            _db.Orders.Update(order);
            _db.SaveChanges();
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
                      .ToList();
        }

        // Get a single order by ID with items
        public Order? GetById(int orderId)
        {
            return _db.Orders
                      .Include(o => o.OrderItems)
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

        // 🔹 NEW: helper method to calculate totals consistently
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
    }
}

