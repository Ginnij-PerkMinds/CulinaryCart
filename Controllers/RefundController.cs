using CulinaryCart.CulinaryCartBAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefundsController : ControllerBase
    {
        private readonly RefundsBAL _bal;

        public RefundsController(RefundsBAL bal)
        {
            _bal = bal;
        }

        [HttpGet("all")]
        public IActionResult GetAllRefunds() => Ok(_bal.GetAllRefunds());

        [HttpGet("pending")]
        public IActionResult GetPendingRefunds() => Ok(_bal.GetRefundsByStatus("Pending"));

        [HttpGet("accepted")]
        public IActionResult GetAcceptedRefunds() => Ok(_bal.GetRefundsByStatus("Accepted"));

        [HttpGet("rejected")]
        public IActionResult GetRejectedRefunds() => Ok(_bal.GetRefundsByStatus("Rejected"));

        [HttpGet("details/{id}")]
        public IActionResult GetRefundDetails(int id)
        {
            var details = _bal.GetRefundDetails(id);
            if (details == null) return NotFound(new { success = false, message = "Refund not found" });
            return Ok(details);
        }

        [HttpPost("{id}/accept")]
        public IActionResult AcceptRefund(int id, [FromBody] string? remarks)
        {
            if (_bal.UpdateRefundStatus(id, "Accepted", remarks))
                return Ok(new { success = true, message = "Refund accepted successfully" });

            return BadRequest(new { success = false, message = "Failed to accept refund" });
        }

        [HttpPost("{id}/reject")]
        public IActionResult RejectRefund(int id, [FromBody] string remarks)
        {
            if (_bal.UpdateRefundStatus(id, "Rejected", remarks))
                return Ok(new { success = true, message = "Refund rejected successfully" });

            return BadRequest(new { success = false, message = "Failed to reject refund. Remarks required." });
        }
    }
}

