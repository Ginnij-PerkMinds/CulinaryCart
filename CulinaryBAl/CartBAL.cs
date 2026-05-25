using CulinaryCart.Controllers;
using CulinaryCart.CulinaryDal;
using CulinaryCart.Model;
using System;
using System.Linq;

namespace CulinaryCart.CulinaryBAl;

public class CartBAL
{
    private readonly MenuDAL _menuDal;
    private readonly OrderHistoryDAL _orderHistoryDal;
    

    public CartBAL(MenuDAL menuDal, OrderHistoryDAL orderHistoryDal)
    {
        _menuDal = menuDal;
        _orderHistoryDal = orderHistoryDal;
    }

    public decimal CalculateFinalPrice(Menu menuItem, int qty)
    {
        if (menuItem == null) return 0;

        decimal basePrice = menuItem.Price * qty;
        decimal finalprice = basePrice;

        if (!string.IsNullOrEmpty(menuItem.Offers))
        {
            string offer = menuItem.Offers.ToUpper();

            if (offer.EndsWith("% OFF"))
            {
                string percentString = offer.ToUpper()
                                .Replace(" OFF", "")
                                .Replace("%", "");
                var percent = Convert.ToDecimal(percentString);
                finalprice = basePrice - ((basePrice/100) * percent);
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

    public void AddItem(int foodItemId, int qty)
    {
        var menuItem = _menuDal.GetItem(foodItemId);

        var finalPrice = CalculateFinalPrice(menuItem, qty);

        var entry = new OrderHistory
        {
            FoodItemID = foodItemId,
            FoodItemName = menuItem.FoodItemName,
            Quantity = qty,
            Price = menuItem.Price,
            FinalPrice = finalPrice,
            Status = CulinaryCartConstants.Status.InCart,
            OrderDate = DateTime.Now
        };
        _orderHistoryDal.Add(entry);
    }

    public void UpdateItem(int foodItemId, int qty)
    {
        var items = _orderHistoryDal.GetAll();
        var item = items.FirstOrDefault(h => h.FoodItemID == foodItemId &&
                                            h.Status == CulinaryCartConstants.Status.InCart);

        if (item != null)
        {
            var menuItem = _menuDal.GetItem(foodItemId);
            item.Quantity = qty;
            item.FinalPrice = CalculateFinalPrice(menuItem, qty);
            _orderHistoryDal.Update(item);
        }
    }

    public void DeleteItem(int foodItemId)
    {
        var items = _orderHistoryDal.GetAll();
        var item = items.FirstOrDefault(h => h.FoodItemID == foodItemId &&
                                            h.Status == CulinaryCartConstants.Status.InCart);
        if (item != null)
        {
            _orderHistoryDal.Delete(item);
        }
    }

    public decimal CalculateCartTotal()
    {
        var items = _orderHistoryDal.GetAll();
        return items.Where(h => h.Status == CulinaryCartConstants.Status.InCart)
                    .Sum(h => h.FinalPrice);
    }

    public void Checkout()
    {
        var items = _orderHistoryDal.GetAll().Where(h =>
                    h.Status == CulinaryCartConstants.Status.InCart).ToList();

        foreach (var item in items)
        {
            item.Status = CulinaryCartConstants.Status.CheckedOut;
            _orderHistoryDal.Update(item);
        }
    }
}

