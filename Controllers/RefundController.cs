using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;



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
        public IActionResult AcceptRefund(int id, [FromBody] AcceptRefundDto dto)
        {
            if (_bal.UpdateRefundStatus(id, "Accepted", dto.Remarks, dto.RefundAmount))
                return Ok(new { success = true, message = "Refund accepted successfully" });

            return BadRequest(new { success = false, message = "Failed to accept refund" });
        }

        [HttpPost("{id}/reject")]
        public IActionResult RejectRefund(int id, [FromBody] RejectRefundDto dto)
        {
            if (_bal.UpdateRefundStatus(id, "Rejected", dto.Remarks, dto.RefundAmount))
                return Ok(new { success = true, message = "Refund rejected successfully" });

            return BadRequest(new { success = false, message = "Failed to reject refund. Remarks required." });
        }

        //[HttpPost("claim")]
        //[Authorize]
        //public IActionResult ClaimRefund([FromForm] RefundClaimDto dto)
        //{
        //    var userIdClaim = User.FindFirst("UserId");
        //    if (userIdClaim == null) return Unauthorized(new { Message = "UserId claim not found in token" });
        //    int userId = int.Parse(userIdClaim.Value);

        //    string? savedFilePath = null;
        //    if (dto.ProofFile != null && dto.ProofFile.Length > 0)
        //    {
        //        var uploadsFolder = Path.Combine("wwwroot", "refunds");
        //        Directory.CreateDirectory(uploadsFolder);

        //        var safeFileName = Path.GetFileName(dto.ProofFile.FileName);

        //        var fileName = $"{Guid.NewGuid()}_{dto.ProofFile.FileName}";
        //        var filePath = Path.Combine(uploadsFolder, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            dto.ProofFile.CopyTo(stream);
        //        }

        //        savedFilePath = $"/refunds/{fileName}";
        //    }

        //    var itemIds = Request.Form["ItemIds"].Select(int.Parse).ToList();
        //    Console.WriteLine($"[RefundsController.ClaimRefund] " +
        //          $"OrderId={dto.OrderId}, RefundAmount={dto.RefundAmount}, " +
        //          $"ItemIds={string.Join(",", itemIds)}");


        //    //var success = _bal.ClaimRefund(userId, dto.OrderId, dto.ItemId, dto.Remarks, savedFilePath);
        //    var success = _bal.ClaimRefund(
        //                  userId,
        //                  dto.OrderId,
        //                  itemIds,
        //                  dto.Remarks,
        //                  savedFilePath,
        //                  dto.RefundAmount
        //                  );

        //    if (!success) 
        //       { 
        //       return BadRequest(new { Message = "Refund already claimed for this order or not eligible." }); 
        //       }

        //    return Ok(new { success = true, message = "Refund request submitted." });
        //}
        [HttpPost("claim")]
        [Authorize]
        public async Task<IActionResult> ClaimRefund([FromForm] int orderId,
                                             [FromForm] decimal refundAmount,
                                             [FromForm] List<IFormFile> proofFiles,
                                             [FromForm] string itemsJson)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized(new { Message = "UserId claim not found in token" });
            int userId = int.Parse(userIdClaim.Value);

            // Deserialize items JSON (sent from Angular)
            //var items = JsonConvert.DeserializeObject<List<RefundItemRequestDto>>(itemsJson);
            var items = JsonSerializer.Deserialize<List<RefundItemRequestDto>>(itemsJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            // Save each proof file and update path
            foreach (var file in proofFiles)
            {
                var uploadsFolder = Path.Combine("wwwroot", "refunds");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var item = items.FirstOrDefault(i => file.FileName.Contains(i.FoodItemId.ToString()));
                if (item != null)
                    item.ProofImage = $"/refunds/{fileName}";
            }

            var success = _bal.ClaimRefund(userId, orderId, items, refundAmount);
            if (!success) return BadRequest(new { Message = "Refund already claimed or not eligible." });

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

