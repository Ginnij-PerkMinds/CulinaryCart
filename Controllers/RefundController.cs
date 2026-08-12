using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using Microsoft.AspNetCore.Authorization;
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

        //[HttpPost("claim")]
        //[Authorize] // user must be logged in
        //public IActionResult ClaimRefund([FromForm] RefundClaimDto dto)
        //{
        //    var userIdClaim = User.FindFirst("UserId");
        //    if (userIdClaim == null) return Unauthorized(new { Message = "UserId claim not found in token" });
        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = _bal.ClaimRefund(userId, dto.OrderId, dto.ItemId, dto.Remarks, dto.ProofFile?.FileName);
        //    if (!success) return BadRequest(new { Message = "Refund not eligible or invalid order." });

        //    return Ok(new { success = true, message = "Refund request submitted." });
        //}
        [HttpPost("claim")]
        [Authorize]
        public IActionResult ClaimRefund([FromForm] RefundClaimDto dto)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized(new { Message = "UserId claim not found in token" });
            int userId = int.Parse(userIdClaim.Value);

            string? savedFilePath = null;
            if (dto.ProofFile != null && dto.ProofFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "refunds");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}_{dto.ProofFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    dto.ProofFile.CopyTo(stream);
                }

                savedFilePath = $"/refunds/{fileName}";
            }

            var success = _bal.ClaimRefund(userId, dto.OrderId, dto.ItemId, dto.Remarks, savedFilePath);
            if (!success) return BadRequest(new { Message = "Refund not eligible or invalid order." });

            return Ok(new { success = true, message = "Refund request submitted." });
        }


        [HttpGet("my-refunds")]
        [Authorize]
        public IActionResult GetMyRefunds()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized(new { Message = "UserId claim not found in token" });
            int userId = int.Parse(userIdClaim.Value);

            var refunds = _bal.GetUserRefunds(userId);
            return Ok(refunds);
        }

    }
}

