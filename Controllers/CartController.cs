using CulinaryCart.CulinaryBAl;
using CulinaryCart.CulinaryDal;
using CulinaryCart.Model;   
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

        [HttpGet("menu")]
        public IActionResult GetMenu() => Ok(_menuDal.GetAllMenuItems());

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
            _cartBal.DeleteItem(foodItemId);
            return Ok(CulinaryCartConstants.Messages.ItemRemoved);
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            
            _cartBal.Checkout();
            return Ok(new { Message = CulinaryCartConstants.Messages.OrderPlaced});
        }
    }

}
