using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;

public class MyOrdersBAL
{
    private readonly OrderHistoryDAL _dal;

    public MyOrdersBAL(OrderHistoryDAL dal)
    {
        _dal = dal;
    }

    public List<MyOrderDto> GetUserOrders(int userId)
    {
        return _dal.GetCompletedOrdersByUser(userId).Select(MapToDto).ToList();
    }

    public MyOrderDetailsDto? GetUserOrderDetails(int userId, int orderId)
    {
        var order = _dal.GetByUser(userId)
                        .FirstOrDefault(o => o.OrderId == orderId &&
                                             (o.Status == CulinaryCartConstants.Status.CheckedOut ||
                                              o.Status == CulinaryCartConstants.Status.Success));
        return order == null ? null : MapToDetailsDto(order);
    }

    private MyOrderDto MapToDto(Order o) => new MyOrderDto
    {
        OrderId = o.OrderId,
        OrderDate = o.OrderDate,
        FinalAmount = o.FinalAmount,
        OrderStatus = o.OrderStatus,
        AppliedPromoCode = o.AppliedPromoCode,
        Remarks = o.Remarks,
        RefundStatus = o.RefundStatus,
        RefundImage = o.RefundImage,
        RefundUserRemarks = o.RefundUserRemarks
    };

    private MyOrderDetailsDto MapToDetailsDto(Order o) => new MyOrderDetailsDto
    {
        OrderId = o.OrderId,
        OrderDate = o.OrderDate,
        BaseAmount = o.BaseAmount,
        PromoDiscount = o.PromoDiscount,
        HandlingFee = o.HandlingFee,
        DeliveryFee = o.DeliveryFee,
        TaxAmount = o.TaxAmount,
        FinalAmount = o.FinalAmount,
        OrderStatus = o.OrderStatus,
        AppliedPromoCode = o.AppliedPromoCode,
        Remarks = o.Remarks,
        RefundStatus = o.RefundStatus,
        RefundImage = o.RefundImage,
        RefundUserRemarks = o.RefundUserRemarks,
        OrderItems = o.OrderItems.Select(i => new MyOrderItemDto
        {
            FoodItemId = i.FoodItemId,
            FoodItemName = i.FoodItemName,
            Quantity = i.Quantity,
            FinalPrice = i.FinalPrice
        }).ToList()
    };
}