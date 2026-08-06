using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class OrdersBAL
    {
        private readonly OrderHistoryDAL _dal;

        public OrdersBAL(OrderHistoryDAL dal)
        {
            _dal = dal;
        }

        // Get all orders
        public List<OrderDto> GetAllOrders()
        {
            return _dal.GetAll().Select(MapToDto).ToList();
        }

        // Get orders by status
        public List<OrderDto> GetOrdersByStatus(string status)
        {
            return _dal.GetOrdersByOrderStatus(status).Select(MapToDto).ToList();
        }

        private string FlattenAddress(Address address)
        {
            if (address == null) return string.Empty;

            return $"{address.HouseNo}, {address.Locality}, {address.Landmark}, " +
                   $"{address.City}, {address.District}, {address.Pincode}, {address.State}";
        }

        // Get single order details
        public OrderDetailsDto? GetOrderDetails(int id)
        {
            var order = _dal.GetById(id);
            if (order == null) return null;

            return new OrderDetailsDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Username = order.User?.Name,                 // adjust to your User entity
                Address = FlattenAddress(order.User?.Address),    // flatten Address object
                PhoneNo = order.User?.PhoneNo,
                BaseAmount = order.BaseAmount,
                PromoDiscount = order.PromoDiscount,
                HandlingFee = order.HandlingFee,
                DeliveryFee = order.DeliveryFee,
                TaxAmount = order.TaxAmount,
                FinalAmount = order.FinalAmount,
                AppliedPromoCode = order?.AppliedPromoCode,
                OrderStatus = order.OrderStatus,
                Remarks = order?.Remarks,
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    FoodItemId = i.FoodItemId,
                    FoodItemName = i.FoodItemName,
                    Quantity = i.Quantity,
                    FinalPrice = i.FinalPrice
                }).ToList()
            };
        }

        // Update order status
        public bool UpdateOrderStatus(int id, string status, string? remarks)
        {
            if (status == "Rejected" && string.IsNullOrWhiteSpace(remarks))
                return false;

            return _dal.UpdateOrderStatus(id, status, remarks);
        }

        // Helper: map EF entity to DTO
        private OrderDto MapToDto(Order o)
        {
            return new OrderDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Username = o.User?.Name,
                Address = FlattenAddress(o.User?.Address),
                PhoneNo = o.User?.PhoneNo,
                FinalAmount = o.FinalAmount,
                OrderStatus = o.OrderStatus,
                AppliedPromoCode = o.AppliedPromoCode,
                Remarks = o.Remarks
            };
        }
    }
}


