using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("add")]
        public IActionResult AddToCart(int foodItemId, int qty)
        {
            var menuItem = _menuDal.GetItem(foodItemId);

            if (menuItem == null)
            {
                return NotFound($"Menu item with ID {foodItemId} not found.");
            }

            var finalPrice = _cartBal.CalculateFinalPrice(menuItem, qty);
            _cartBal.AddItem(foodItemId, qty);
            return Ok(CulinaryCartConstants.Messages.ItemAdded);
        }

        [HttpPut("update")]
        public IActionResult UpdateCartItem(int foodItemId, int qty)
        {
            var menuItem = _menuDal.GetItem(foodItemId);

            if (menuItem == null)
            {
                return NotFound($"Menu item with ID {foodItemId} not found.");
            }

            var finalPrice = _cartBal.CalculateFinalPrice(menuItem, qty);
            _cartBal.UpdateItem(foodItemId, qty);
            return Ok(CulinaryCartConstants.Messages.ItemUpdated);
        }

        [HttpDelete("delete/{foodItemId}")]
        public IActionResult DeleteCartItem(int foodItemId)
        {
            var item = _menuDal.GetItem(foodItemId); 
            if (item == null)
            {
                return NotFound($"Cart item with ID {foodItemId} not found.");
            }

            _cartBal.DeleteItem(foodItemId);
            return Ok(CulinaryCartConstants.Messages.ItemRemoved);
        }

        [HttpGet]
        [Route("ViewCart")]
        public IActionResult ViewCart()
        {
            var items = _cartBal.GetCartItems();

            if (items == null || !items.Any())
            {
                return Ok(new { Message = CulinaryCartConstants.Messages.CartIsEmpty });
            }

            return Ok(items);
        }
        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            _cartBal.Checkout();   
            return Ok(new { Message = CulinaryCartConstants.Messages.CheckoutSuccessful });
        }

        [HttpGet("order-stats")]
        public IActionResult GetOrderStats()
        {
            var orders = _orderHistoryDal.GetAll();

            var totalOrders = orders.Count;

            var totalRevenue = orders.Sum(o => o.FinalPrice);

            var topItem = orders
                .GroupBy(o => o.FoodItemName)
                .Select(g => new
                {
                    FoodItemName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .ToList();
            //.FirstOrDefault();

            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                //TopOrderedItem = topItem?.FoodItemName,
                //TopOrderedQuantity = topItem?.TotalQuantity
                TopItems = topItem
            });
        }

        [HttpGet("revenue-by-date")]
        public IActionResult GetRevenueByDate()
        {
            var orders = _orderHistoryDal.GetAll();

            var revenueByDate = orders
                .GroupBy(o => o.OrderDate.Date)   // group by just the date
                .Select(g => new {
                    Date = g.Key.ToString("dd/MM"),
                    TotalRevenue = g.Sum(x => x.FinalPrice),
                    DateValue=g.Key
                })
                .OrderBy(g => g.DateValue)
                .ToList();

            return Ok(revenueByDate);
        }

    }
}