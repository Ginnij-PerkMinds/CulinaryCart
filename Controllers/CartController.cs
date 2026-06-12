using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartBAL _cartBal;
        private readonly MenuDAL _menuDal;

        public CartController(CartBAL cartBal, MenuDAL menuDal)
        {
            _cartBal = cartBal;
            _menuDal = menuDal;
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
            var item = _menuDal.GetItem(foodItemId); // or _cartDal.GetItem
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
                return Ok(new { Message = "Cart is empty" });
            }

            return Ok(items);
        }

    }

}
