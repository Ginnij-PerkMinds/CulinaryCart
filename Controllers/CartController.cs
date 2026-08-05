using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Models.DTO.CulinaryCart.CulinaryCartBAL.DTOs;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartBAL _cartBal;
        private readonly MenuDAL _menuDal;
        private readonly OrderHistoryDAL _orderHistoryDal;
        private readonly PromocodeDAL _promoDal;
        private readonly ChargeDAL _chargeDal;

        public CartController(CartBAL cartBal, MenuDAL menuDal, OrderHistoryDAL orderHistoryDal, PromocodeDAL promoDal, ChargeDAL chargeDal)
        {
            _cartBal = cartBal;
            _menuDal = menuDal;
            _orderHistoryDal = orderHistoryDal;
            _promoDal = promoDal;
            _chargeDal = chargeDal;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                throw new UnauthorizedAccessException(CulinaryCartConstants.Messages.UserIdClaimMissing);
            return int.Parse(userIdClaim.Value);
        }

        // Add item to cart
        [HttpPost("add")]
        public IActionResult AddToCart(int foodItemId, int qty)
        {
            int userId = GetUserIdFromToken();
            var menuItem = _menuDal.GetItem(foodItemId);

            if (menuItem == null)
                return NotFound($"Menu item with ID {foodItemId} not found.");

            _cartBal.AddItem(userId, foodItemId, qty);
            return Ok(CulinaryCartConstants.Messages.ItemAdded);
        }

        // Update item in cart
        [HttpPut("update")]
        public IActionResult UpdateCartItem(int foodItemId, int qty)
        {
            int userId = GetUserIdFromToken();
            var menuItem = _menuDal.GetItem(foodItemId);

            if (menuItem == null)
                return NotFound($"Menu item with ID {foodItemId} not found.");

            _cartBal.UpdateItem(userId, foodItemId, qty);
            return Ok(CulinaryCartConstants.Messages.ItemUpdated);
        }

        // Delete item from cart
        [HttpDelete("delete/{foodItemId}")]
        public IActionResult DeleteCartItem(int foodItemId)
        {
            int userId = GetUserIdFromToken();
            var menuItem = _menuDal.GetItem(foodItemId);

            if (menuItem == null)
                return NotFound($"Cart item with ID {foodItemId} not found.");

            _cartBal.DeleteItem(userId, foodItemId);
            return Ok(CulinaryCartConstants.Messages.ItemRemoved);
        }

        // View cart with breakdown
        [HttpGet("view")]
        public IActionResult ViewCart([FromQuery] string promoCode = null)
        {
            int userId = GetUserIdFromToken();
            var items = _cartBal.GetCartItems(userId);

            if (items == null || !items.Any())
                return Ok(new CartResponseDto { Message = CulinaryCartConstants.Messages.CartisEmpty });

            var baseAmount = items.Sum(i => i.FinalPrice);

            //  Get full calculation result object
            var calcResult = _cartBal.CalculateFinalAmount(baseAmount, promoCode);

            return Ok(new CartResponseDto
            {
                Items = items.Select(i => new CartItemDto
                {
                    FoodItemId = i.FoodItemId,
                    FoodItemName = i.FoodItemName,
                    Quantity = i.Quantity,
                    FinalPrice = i.FinalPrice
                }).ToList(),
                BaseAmount = calcResult.BaseAmount,
                PromoDiscount = calcResult.PromoDiscount,
                Charges = new List<CartChargeDto>
        {
            new CartChargeDto { ChargeType = "HandlingFee", Value = calcResult.HandlingFee },
            new CartChargeDto { ChargeType = "DeliveryFee", Value = calcResult.DeliveryFee },
            new CartChargeDto { ChargeType = "Tax", Value = calcResult.TaxAmount }
        },
                FinalAmount = calcResult.FinalAmount,
                AppliedPromoCode = calcResult.AppliedPromoCode,
                Message = "Cart retrieved successfully."
            });
        }

        // Checkout with breakdown
        [HttpPost("checkout")]
        public IActionResult Checkout([FromQuery] string promoCode = null)
        {
            int userId = GetUserIdFromToken();
            var order = _cartBal.Checkout(userId);

            if (order == null)
                return BadRequest("Cart is empty.");

            var charges = new List<CartChargeDto>
            {
                new CartChargeDto { ChargeType = "HandlingFee", Value = order.HandlingFee },
                new CartChargeDto { ChargeType = "DeliveryFee", Value = order.DeliveryFee },
                new CartChargeDto { ChargeType = "Tax", Value = order.TaxAmount }
            };

            return Ok(new CartResponseDto
            {
                Items = order.OrderItems.Select(i => new CartItemDto
                {
                    FoodItemId = i.FoodItemId,
                    FoodItemName = i.FoodItemName,
                    Quantity = i.Quantity,
                    FinalPrice = i.FinalPrice
                }).ToList(),
                BaseAmount = order.BaseAmount,
                PromoDiscount = order.PromoDiscount,
                Charges = charges,
                FinalAmount = order.FinalAmount,
                AppliedPromoCode = order.AppliedPromoCode,
                Message = CulinaryCartConstants.Messages.CheckoutSuccessful
            });
        }

        // Order history by user
        [HttpGet("order-history/{userId}")]
        public IActionResult GetOrderHistoryByUser(int userId)
        {
            var orders = _orderHistoryDal.GetByUser(userId)
                .Where(o => o.Status == CulinaryCartConstants.Status.CheckedOut)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.BaseAmount,
                    o.PromoDiscount,
                    o.HandlingFee,
                    o.DeliveryFee,
                    o.TaxAmount,
                    o.FinalAmount,
                    o.AppliedPromoCode,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.FoodItemName,
                        i.Quantity,
                        i.FinalPrice
                    })
                })
                .ToList();

            if (!orders.Any())
                return Ok(new { Message = $"No past orders found for user {userId}." });

            return Ok(orders);
        }

        // My orders
        [HttpGet("my-orders")]
        public IActionResult GetMyOrders()
        {
            int userId = GetUserIdFromToken();
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            var orders = _orderHistoryDal.GetByUser(userId)
                .Where(o => o.Status == CulinaryCartConstants.Status.CheckedOut)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    OrderDate = TimeZoneInfo.ConvertTimeFromUtc(o.OrderDate, indiaTimeZone),
                    o.BaseAmount,
                    o.PromoDiscount,
                    o.HandlingFee,
                    o.DeliveryFee,
                    o.TaxAmount,
                    o.FinalAmount,
                    o.AppliedPromoCode,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.FoodItemName,
                        i.Quantity,
                        i.FinalPrice
                    })
                })
                .ToList();

            if (!orders.Any())
                return Ok(new { Message = "No past orders found." });

            return Ok(orders);
        }

        // Order stats
        [HttpGet("order-stats")]
        public IActionResult GetOrderStats()
        {
            var orders = _orderHistoryDal.GetCheckedOutOrders();

            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.FinalAmount);

            var topItems = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.FoodItemName)
                .Select(g => new
                {
                    FoodItemName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .ToList();

            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TopItems = topItems
            });
        }

        // Revenue by date
        [HttpGet("revenue-by-date")]
        public IActionResult GetRevenueByDate()
        {
            var orders = _orderHistoryDal.GetCheckedOutOrders();

            var revenueByDate = orders
               .GroupBy(o => o.OrderDate.Date)
               .Select(g => new
               {
                   Date = g.Key.ToString("dd/MM"),
                   TotalRevenue = g.Sum(o => o.FinalAmount),
                   DateValue = g.Key
               })
               .OrderBy(g => g.DateValue)
               .ToList();

            return Ok(revenueByDate);
        }

        // ✅ Apply Promo Code to Cart
        [HttpPost("apply-promo")]
        public IActionResult ApplyPromo([FromBody] ApplyPromoRequest request)
        {
            int userId = GetUserIdFromToken();

            if (string.IsNullOrWhiteSpace(request?.PromoCode))
                return BadRequest(new { message = "Promo code is required." });

            _cartBal.ApplyPromo(userId, request.PromoCode);

            // Get updated cart with promo applied
            var items = _cartBal.GetCartItems(userId);

            if (items == null || !items.Any())
                return Ok(new CartResponseDto { Message = CulinaryCartConstants.Messages.CartisEmpty });

            var baseAmount = items.Sum(i => i.FinalPrice);
            var calcResult = _cartBal.CalculateFinalAmount(baseAmount, request.PromoCode);

            return Ok(new CartResponseDto
            {
                Items = items.Select(i => new CartItemDto
                {
                    FoodItemId = i.FoodItemId,
                    FoodItemName = i.FoodItemName,
                    Quantity = i.Quantity,
                    FinalPrice = i.FinalPrice
                }).ToList(),
                BaseAmount = calcResult.BaseAmount,
                PromoDiscount = calcResult.PromoDiscount,
                Charges = new List<CartChargeDto>
                {
                    new CartChargeDto { ChargeType = "HandlingFee", Value = calcResult.HandlingFee },
                    new CartChargeDto { ChargeType = "DeliveryFee", Value = calcResult.DeliveryFee },
                    new CartChargeDto { ChargeType = "Tax", Value = calcResult.TaxAmount }
                },
                FinalAmount = calcResult.FinalAmount,
                AppliedPromoCode = calcResult.AppliedPromoCode,
                Message = calcResult.AppliedPromoCode != null 
                    ? $"Promo code '{calcResult.AppliedPromoCode}' applied successfully! Discount: ₹{calcResult.PromoDiscount}" 
                    : "Promo code is invalid or expired."
            });
        }

    }
}
