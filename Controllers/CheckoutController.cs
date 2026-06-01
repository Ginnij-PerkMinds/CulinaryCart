using CulinaryCart.CulinaryCartBAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly CartBAL _cartBal;

        public CheckoutController(CartBAL cartBal)
        {
            _cartBal = cartBal;
        }

        // Place order (checkout)
        [HttpPost("placeorder")]
        public IActionResult PlaceOrder()
        {
            _cartBal.Checkout();
            return Ok(new { Message = "Order placed successfully" });
        }
    }
}
