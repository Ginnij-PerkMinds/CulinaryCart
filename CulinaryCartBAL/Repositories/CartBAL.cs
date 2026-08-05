using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO.CulinaryCart.CulinaryCartBAL.DTOs;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class CartBAL
    {
        private readonly MenuDAL _menuDal;
        private readonly OrderHistoryDAL _orderHistoryDal;
        private readonly ChargeDAL _chargeDal;
        private readonly PromocodeDAL _promoDal;

        public CartBAL(MenuDAL menuDal, OrderHistoryDAL orderHistoryDal, ChargeDAL chargeDal, PromocodeDAL promoDal)
        {
            _menuDal = menuDal;
            _orderHistoryDal = orderHistoryDal;
            _chargeDal = chargeDal;
            _promoDal = promoDal;
        }

        // Business logic for offers/discounts on menu items
        public decimal CalculateFinalPrice(Menu menuItem, int qty)
        {
            if (menuItem == null) return 0;

            decimal basePrice = menuItem.Price * qty;
            decimal finalPrice = basePrice;

            if (!string.IsNullOrWhiteSpace(menuItem.Offers))
            {
                string offer = menuItem.Offers.Trim().ToUpper();

                if (offer.EndsWith("%"))
                {
                    string percentString = offer.Replace("%", "").Trim();
                    if (decimal.TryParse(percentString, out var percent))
                        finalPrice = basePrice - ((basePrice / 100) * percent);
                }
                else if (offer == "BUY1GET1")
                {
                    int payableQty = (qty + 1) / 2;
                    finalPrice = menuItem.Price * payableQty;
                }
                else if (offer == "BUY2GET1")
                {
                    int freeItems = qty / 3;
                    finalPrice = basePrice - (freeItems * menuItem.Price);
                }
                else if (offer == "BUY3GET1")
                {
                    int freeItems = qty / 4;
                    finalPrice = basePrice - (freeItems * menuItem.Price);
                }
            }

            return finalPrice;
        }

        // Calculate final amount with promo + charges
        public CartCalculationResult CalculateFinalAmount(decimal baseAmount, string promoCode)
        {
            decimal promoDiscount = 0;
            string appliedPromoCode = null;
            bool freeDelivery = false;

            Console.WriteLine($"[CalculateFinalAmount] BaseAmount: {baseAmount}, PromoCode: {promoCode}");

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promo = _promoDal.GetAllPromocodes()
                    .FirstOrDefault(p => p.PromoCodeName.Equals(promoCode, StringComparison.OrdinalIgnoreCase));

                Console.WriteLine($"[CalculateFinalAmount] Promo found: {promo != null}");

                if (promo != null)
                {
                    Console.WriteLine($"[CalculateFinalAmount] Promo details - IsActive: {promo.IsActive}, UsageCount: {promo.UsageCount}, Criteria: {promo.Criteria}, Amount: {promo.Amount}");
                }

                if (promo != null && promo.IsActive && promo.UsageCount > 0 && baseAmount >= promo.Criteria)
                {
                    var amount = promo.Amount ?? 0m;
                    promoDiscount = amount >= 1 ? amount : baseAmount * amount;
                    appliedPromoCode = promo.PromoCodeName;

                    Console.WriteLine($"[CalculateFinalAmount] ✅ Promo applied - PromoDiscount: {promoDiscount}, AppliedCode: {appliedPromoCode}");

                    if (promo.FreeDelivery)
                        freeDelivery = true;
                }
                else
                {
                    if (promo == null)
                        Console.WriteLine($"[CalculateFinalAmount] ❌ Promo not found");
                    else if (!promo.IsActive)
                        Console.WriteLine($"[CalculateFinalAmount] ❌ Promo not active");
                    else if (promo.UsageCount <= 0)
                        Console.WriteLine($"[CalculateFinalAmount] ❌ Promo usage count exhausted");
                    else if (baseAmount < promo.Criteria)
                        Console.WriteLine($"[CalculateFinalAmount] ❌ BaseAmount ({baseAmount}) < Criteria ({promo.Criteria})");
                }
            }

            decimal handlingFee = 0, deliveryFee = 0, taxAmount = 0;
            var charges = _chargeDal.GetAllCharges().Where(c => c.IsActive).ToList();

            foreach (var c in charges)
            {
                switch (c.ChargeType.ToUpper())
                {
                    case "HANDLING FEE":
                        handlingFee += baseAmount * c.Value;
                        break;
                    case "DELIVERY FEE":
                        deliveryFee += freeDelivery ? 0 : baseAmount * c.Value;
                        break;
                    case "COLLECTIBLE TAX":
                        taxAmount += baseAmount * c.Value;
                        break;
                }
            }

            var finalAmount = baseAmount - promoDiscount + handlingFee + deliveryFee + taxAmount;

            Console.WriteLine($"[CalculateFinalAmount] Final - HandlingFee: {handlingFee}, DeliveryFee: {deliveryFee}, TaxAmount: {taxAmount}, FinalAmount: {finalAmount}");

            return new CartCalculationResult
            {
                BaseAmount = baseAmount,
                PromoDiscount = promoDiscount,
                AppliedPromoCode = appliedPromoCode,
                HandlingFee = handlingFee,
                DeliveryFee = deliveryFee,
                TaxAmount = taxAmount,
                FinalAmount = finalAmount
            };
        }

        // 🔹 Helper to persist charges + promo + final amount into DB
        private void UpdateOrderTotals(Order order, string promoCode = null)
        {
            order.BaseAmount = order.OrderItems.Sum(i => i.FinalPrice);

            var calc = CalculateFinalAmount(order.BaseAmount, promoCode ?? order.AppliedPromoCode);

            order.PromoDiscount = calc.PromoDiscount;
            order.AppliedPromoCode = calc.AppliedPromoCode;
            order.HandlingFee = calc.HandlingFee;
            order.DeliveryFee = calc.DeliveryFee;
            order.TaxAmount = calc.TaxAmount;
            order.FinalAmount = calc.FinalAmount;
            order.TotalAmount = calc.FinalAmount;
        }

        // Add item to cart
        public void AddItem(int userId, int foodItemId, int qty)
        {
            var menuItem = _menuDal.GetItem(foodItemId);
            if (menuItem == null) return;

            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = CulinaryCartConstants.Status.InCart,
                    OrderItems = new List<OrderItem>()
                };
                _orderHistoryDal.Add(order);
            }

            var existingItem = order.OrderItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += qty;
                existingItem.FinalPrice = CalculateFinalPrice(menuItem, existingItem.Quantity);
            }
            else
            {
                order.OrderItems.Add(new OrderItem
                {
                    FoodItemId = foodItemId,
                    FoodItemName = menuItem.FoodItemName,
                    Quantity = qty,
                    Price = menuItem.Price,
                    FinalPrice = CalculateFinalPrice(menuItem, qty)
                });
            }

            UpdateOrderTotals(order);
            _orderHistoryDal.Update(order);
        }

        // Update item in cart
        public void UpdateItem(int userId, int foodItemId, int qty)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null) return;

            var item = order.OrderItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (item != null)
            {
                var menuItem = _menuDal.GetItem(foodItemId);
                if (menuItem != null)
                {
                    item.Quantity = qty;
                    item.FinalPrice = CalculateFinalPrice(menuItem, qty);
                }
            }

            UpdateOrderTotals(order);
            _orderHistoryDal.Update(order);
        }

        // Delete item from cart
        public void DeleteItem(int userId, int foodItemId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null) return;

            var item = order.OrderItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (item != null)
            {
                order.OrderItems.Remove(item);
            }

            UpdateOrderTotals(order);
            _orderHistoryDal.Update(order);
        }

        // Apply promo explicitly
        public void ApplyPromo(int userId, string promoCode)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            Console.WriteLine($"[CartBAL.ApplyPromo] UserId: {userId}, PromoCode: {promoCode}, Order found: {order != null}");

            if (order == null) 
            {
                Console.WriteLine($"[CartBAL.ApplyPromo] No InCart order found for userId {userId}");
                return;
            }

            Console.WriteLine($"[CartBAL.ApplyPromo] Before UpdateOrderTotals - OrderId: {order.OrderId}, BaseAmount: {order.BaseAmount}");

            UpdateOrderTotals(order, promoCode);

            Console.WriteLine($"[CartBAL.ApplyPromo] After UpdateOrderTotals - AppliedPromoCode: {order.AppliedPromoCode}, PromoDiscount: {order.PromoDiscount}, FinalAmount: {order.FinalAmount}");

            _orderHistoryDal.Update(order);

            Console.WriteLine($"[CartBAL.ApplyPromo] Order saved to DB - AppliedPromoCode: {order.AppliedPromoCode}");
        }

        // Get cart items
        public IEnumerable<OrderItem> GetCartItems(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            return order?.OrderItems ?? new List<OrderItem>();
        }

        // Clear cart
        public void ClearCart(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order != null)
            {
                order.OrderItems.Clear();
                UpdateOrderTotals(order);
                _orderHistoryDal.Update(order);
            }
        }
        public Order Checkout(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null || !order.OrderItems.Any())
                return null;

            // Reduce stock
            foreach (var item in order.OrderItems)
            {
                var menuItem = _menuDal.GetItem(item.FoodItemId);
                if (menuItem != null)
                {
                    menuItem.RemainingQuantity -= item.Quantity;
                    if (menuItem.RemainingQuantity <= 0)
                    {
                        menuItem.RemainingQuantity = 0;
                        menuItem.InStock = false;
                    }
                    _menuDal.Update(menuItem);
                }
            }

            // ✅ Just finalize status and date
            order.Status = CulinaryCartConstants.Status.CheckedOut;
            order.OrderDate = DateTime.UtcNow;

            _orderHistoryDal.Update(order);

            // Clear cart reference
            this.ClearCart(userId);

            return order;
        }

    }
}