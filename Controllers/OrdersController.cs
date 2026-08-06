using CulinaryCart.CulinaryCartBAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersBAL _bal;

        public OrdersController(OrdersBAL bal)
        {
            _bal = bal;
        }

        // Get all orders
        [HttpGet("all")]
        public IActionResult GetAllOrders()
        {
            return Ok(_bal.GetAllOrders());
        }

        // Get orders by status
        [HttpGet("pending")]
        public IActionResult GetPendingOrders()
        {
            return Ok(_bal.GetOrdersByStatus("Pending"));
        }

        [HttpGet("accepted")]
        public IActionResult GetAcceptedOrders()
        {
            return Ok(_bal.GetOrdersByStatus("Accepted"));
        }

        [HttpGet("rejected")]
        public IActionResult GetRejectedOrders()
        {
            return Ok(_bal.GetOrdersByStatus("Rejected"));
        }

        // Get order details by ID
        [HttpGet("details/{id}")]
        public IActionResult GetOrderDetails(int id)
        {
            var details = _bal.GetOrderDetails(id);
            if (details == null) return NotFound(new { success = false, message = "Order not found" });
            return Ok(details);
        }

        // Accept order
        [HttpPost("{id}/accept")]
        public IActionResult AcceptOrder(int id, [FromBody] string? remarks)
        {
            if (_bal.UpdateOrderStatus(id, "Accepted", remarks))
                return Ok(new { success = true, message = "Order accepted successfully" });

            return BadRequest(new { success = false, message = "Failed to accept order" });
        }

        // Reject order
        [HttpPost("{id}/reject")]
        public IActionResult RejectOrder(int id, [FromBody] string remarks)
        {
            if (_bal.UpdateOrderStatus(id, "Rejected", remarks))
                return Ok(new { success = true, message = "Order rejected successfully" });

            return BadRequest(new { success = false, message = "Failed to reject order. Remarks required." });
        }
    }
}

