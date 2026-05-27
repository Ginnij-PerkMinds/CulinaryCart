using CulinaryCart.Constants;
using CulinaryCart.CulinaryDal;
using CulinaryCart.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryBAl
{
    public class CartBAL
    {
        private readonly CartDAL _cartDal;
        private readonly MenuDAL _menuDal;
        private readonly OrderHistoryDAL _orderHistoryDal;

        public CartBAL(MenuDAL menuDal, OrderHistoryDAL orderHistoryDal)
        {
            _menuDal = menuDal;
            _orderHistoryDal = orderHistoryDal;
        }

        // ✅ Centralized business logic for offers/discounts
        public decimal CalculateFinalPrice(Menu menuItem, int qty)
        {
            if (menuItem == null) return 0;

            decimal basePrice = menuItem.Price * qty;
            decimal finalprice = basePrice;

            if (!string.IsNullOrWhiteSpace(menuItem.Offers))
            {
                string offer = menuItem.Offers.Trim().ToUpper();

                if (offer.EndsWith("%"))
                {
                    string percentString = offer.Replace("%", "").Trim();
                    if (decimal.TryParse(percentString, out var percent))
                    {
                        finalprice = basePrice - ((basePrice / 100) * percent);
                    }
                }
                else if (offer == "BUY1GET1")
                {
                    int payableQty = (qty + 1) / 2;
                    finalprice = menuItem.Price * payableQty;
                }
                else if (offer == "BUY2GET1")
                {
                    int freeItems = qty / 3;
                    finalprice = basePrice - (freeItems * menuItem.Price);
                }
                else if (offer == "BUY3GET1")
                {
                    int freeItems = qty / 4;
                    finalprice = basePrice - (freeItems * menuItem.Price);
                }
            }

            return finalprice;
        }

        // ✅ Add item to cart
        public void AddItem(int foodItemId, int qty)
        {
            var menuItem = _menuDal.GetItem(foodItemId);
            if (menuItem == null) return;

            var existing = _orderHistoryDal.GetAll()
                .FirstOrDefault(h => h.FoodItemID == foodItemId &&
                                     h.Status == CulinaryCartConstants.Status.InCart);

            if (existing != null)
            {
                existing.Quantity += qty;
                existing.FinalPrice = CalculateFinalPrice(menuItem, existing.Quantity);
                _orderHistoryDal.Update(existing);
            }
            else
            {
                var entry = new OrderHistory
                {
                    FoodItemID = foodItemId,
                    FoodItemName = menuItem.FoodItemName,
                    Quantity = qty,
                    Price = menuItem.Price,
                    FinalPrice = CalculateFinalPrice(menuItem, qty),
                    Status = CulinaryCartConstants.Status.InCart,
                    OrderDate = DateTime.Now
                };
                _orderHistoryDal.Add(entry);
            }
        }



        // ✅ Update item in cart
        public void UpdateItem(int foodItemId, int qty)
        {
            var item = _orderHistoryDal.GetAll()
                .FirstOrDefault(h => h.FoodItemID == foodItemId &&
                                     h.Status == CulinaryCartConstants.Status.InCart);

            if (item != null)
            {
                var menuItem = _menuDal.GetItem(foodItemId);
                if (menuItem != null)
                {
                    item.Quantity = qty;
                    item.FinalPrice = CalculateFinalPrice(menuItem, qty);
                    _orderHistoryDal.Update(item);
                }
            }
        }



        // ✅ Delete item from cart
        public void DeleteItem(int foodItemId)
        {
            var item = _orderHistoryDal.GetAll()
                .FirstOrDefault(h => h.FoodItemID == foodItemId &&
                                     h.Status == CulinaryCartConstants.Status.InCart);
            if (item != null)
            {
                _orderHistoryDal.Delete(item);
            }
        }
        


        // ✅ Calculate total cart value
        public decimal CalculateCartTotal()
        {
            return _orderHistoryDal.GetAll()
                .Where(h => h.Status == CulinaryCartConstants.Status.InCart)
                .Sum(h => h.FinalPrice);
        }

        // ✅ Checkout (mark items as checked out)
        public void Checkout()
        {
            var items = _orderHistoryDal.GetAll()
                .Where(h => h.Status == CulinaryCartConstants.Status.InCart)
                .ToList();

            foreach (var item in items)
            {
                item.Status = CulinaryCartConstants.Status.CheckedOut;
                _orderHistoryDal.Update(item);
            }
        }

        // ✅ Implemented: return cart items
        public IEnumerable<OrderHistory> GetCartItems()
        {
            return _orderHistoryDal.GetAll()
                .Where(h => h.Status == CulinaryCartConstants.Status.InCart)
                .ToList();
        }


    }
}


