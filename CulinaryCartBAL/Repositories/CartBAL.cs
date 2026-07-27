using CulinaryCart.CulinaryCartBAL.Constants;
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

        public CartBAL(MenuDAL menuDal, OrderHistoryDAL orderHistoryDal)
        {
            _menuDal = menuDal;
            _orderHistoryDal = orderHistoryDal;
        }

        // Business logic for offers/discounts
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
                    Status = CulinaryCartConstants.Status.InCart
                };

                order.OrderItems.Add(new OrderItem
                {
                    FoodItemId = foodItemId,
                    FoodItemName = menuItem.FoodItemName,
                    Quantity = qty,
                    Price = menuItem.Price,
                    FinalPrice = CalculateFinalPrice(menuItem, qty)
                });

                order.TotalAmount = order.OrderItems.Sum(i => i.FinalPrice);
                _orderHistoryDal.Add(order);   // persist new order
            }
            else
            {
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

                order.TotalAmount = order.OrderItems.Sum(i => i.FinalPrice);
                _orderHistoryDal.Update(order);   // update existing order
            }
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

                order.TotalAmount = order.OrderItems.Sum(i => i.FinalPrice);
                _orderHistoryDal.Update(order);
            }
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
                order.TotalAmount = order.OrderItems.Any()
                    ? order.OrderItems.Sum(i => i.FinalPrice)
                    : 0;

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

        // Calculate total cart value
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
                order.TotalAmount = 0;
                _orderHistoryDal.Update(order);
            }
        }

        // Checkout
        public Order Checkout(int userId)
        {
            var order = _orderHistoryDal.GetByUser(userId)
                .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null || !order.OrderItems.Any())
                return null;

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
            order.TotalAmount = order.OrderItems.Sum(i => i.FinalPrice);

            _orderHistoryDal.Update(order);

            // Clear cart after checkout
            this.ClearCart(userId);

            return order;
        }
    }
}