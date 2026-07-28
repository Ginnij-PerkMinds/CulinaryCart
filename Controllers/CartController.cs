using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartBAL _cartBal;
        private readonly MenuDAL _menuDal;
        private readonly OrderHistoryDAL _orderHistoryDal;

        public CartController(CartBAL cartBal, MenuDAL menuDal, OrderHistoryDAL orderHistoryDal)
        {
            _cartBal = cartBal;
            _menuDal = menuDal;
            _orderHistoryDal = orderHistoryDal;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId");
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

        // View cart
        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            int userId = GetUserIdFromToken();
            var items = _cartBal.GetCartItems(userId);

            if (items == null || !items.Any())
                return Ok(new { Message = CulinaryCartConstants.Messages.CartisEmpty });

            return Ok(items);
        }

        // Checkout
        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            int userId = GetUserIdFromToken();
            var order = _cartBal.Checkout(userId);

            if (order == null)
                return BadRequest("Cart is empty.");

            return Ok(new
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                Message = CulinaryCartConstants.Messages.CheckoutSuccessful
            });
        }

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
                    o.TotalAmount,
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
                    o.TotalAmount,
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
            var totalRevenue = orders.Sum(o => o.TotalAmount);

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
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    DateValue = g.Key
                })
                .OrderBy(g => g.DateValue)
                .ToList();

            return Ok(revenueByDate);
        }
    }
}