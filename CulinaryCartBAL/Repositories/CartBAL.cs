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

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promo = _promoDal.GetAllPromocodes()
                    .FirstOrDefault(p => p.PromoCodeName.Equals(promoCode, StringComparison.OrdinalIgnoreCase));

                if (promo != null && promo.IsActive && promo.UsageCount > 0 && baseAmount >= promo.Criteria)
                {
                    var amount = promo.Amount ?? 0m;
                    promoDiscount = amount >= 1 ? amount : baseAmount * amount;
                    appliedPromoCode = promo.PromoCodeName;

                    if (promo.FreeDelivery)
                        freeDelivery = true;

                    promo.UsageCount -= 1;
                    _promoDal.UpdatePromocode(promo.Id, promo);
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
                    OrderItems = new List<OrderItem>() // ✅ initialize
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

            order.BaseAmount = order.OrderItems.Sum(i => i.FinalPrice);
            order.TotalAmount = order.BaseAmount;
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

            order.BaseAmount = order.OrderItems.Sum(i => i.FinalPrice);
            order.TotalAmount = order.BaseAmount;
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
                order.BaseAmount = order.OrderItems.Any()
                    ? order.OrderItems.Sum(i => i.FinalPrice)
                    : 0;

                order.TotalAmount = order.BaseAmount;
                _orderHistoryDal.Update(order);
            }
        }

        // Get cart items
        public IEnumerable<OrderItem> GetCartItems(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            return order?.OrderItems ?? new List<OrderItem>();
        }

        // Calculate total cart value (without charges/promo)
        public decimal CalculateCartTotal(int userId)
        {
            var items = GetCartItems(userId);
            return items.Sum(i => i.FinalPrice);
        }

        // Clear cart
        public void ClearCart(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order != null)
            {
                order.OrderItems.Clear();
                order.BaseAmount = 0;
                order.TotalAmount = 0;
                _orderHistoryDal.Update(order);
            }
        }

        // Checkout with charges + promo
        public Order Checkout(int userId, string promoCode = null)
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

            order.Status = CulinaryCartConstants.Status.CheckedOut;
            order.OrderDate = DateTime.UtcNow;

            // Base amount
            order.BaseAmount = order.OrderItems.Sum(i => i.FinalPrice);

            // Promo discount
            order.PromoDiscount = 0;
            order.AppliedPromoCode = null;
            bool freeDelivery = false;

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promo = _promoDal.GetAllPromocodes()
                    .FirstOrDefault(p => p.PromoCodeName.Equals(promoCode, StringComparison.OrdinalIgnoreCase));

                if (promo != null && promo.IsActive && promo.UsageCount > 0 && order.BaseAmount >= promo.Criteria)
                {
                    var amount = promo.Amount ?? 0m;
                    order.PromoDiscount = amount >= 1 ? amount : order.BaseAmount * amount;

                    order.AppliedPromoCode = promo.PromoCodeName;

                    if (promo.FreeDelivery)
                        freeDelivery = true;

                    promo.UsageCount -= 1;
                    _promoDal.UpdatePromocode(promo.Id, promo);
                }
            }

            // Charges
            order.HandlingFee = 0;
            order.DeliveryFee = 0;
            order.TaxAmount = 0;

            var charges = _chargeDal.GetAllCharges().Where(c => c.IsActive).ToList();
            foreach (var charge in charges)
            {
                switch (charge.ChargeType.ToUpper())
                {
                    case "HANDLING FEE":
                        order.HandlingFee += order.BaseAmount * charge.Value;
                        break;
                    case "DELIVERY FEE":
                        order.DeliveryFee += freeDelivery ? 0 : order.BaseAmount * charge.Value;
                        break;
                    case "COLLECTIBLE TAX":
                        order.TaxAmount += order.BaseAmount * charge.Value;
                        break;
                }
            }

            // Final total
            order.FinalAmount = order.BaseAmount - order.PromoDiscount
                                + order.HandlingFee + order.DeliveryFee + order.TaxAmount;

            // Keep legacy TotalAmount in sync
            order.TotalAmount = order.FinalAmount;

            _orderHistoryDal.Update(order);

            // Clear cart after checkout
            this.ClearCart(userId);

            return order;
        }
    }
}