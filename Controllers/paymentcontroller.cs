using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace CulinaryCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly OrderHistoryDAL _orderHistoryDal;
        private readonly IConfiguration _configuration;

        public PaymentController(OrderHistoryDAL orderHistoryDal, IConfiguration configuration)
        {
            _orderHistoryDal = orderHistoryDal;
            _configuration = configuration;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("UserId claim missing in token.");
            return int.Parse(userIdClaim.Value);
        }

        // Razorpay order response DTO
        public class RazorpayOrderResponse
        {
            public string id { get; set; }
            public int amount { get; set; }
            public string currency { get; set; }
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = GetUserIdFromToken();

            var order = _orderHistoryDal.GetByUser(userId)
                        .OrderByDescending(o => o.OrderDate)
                        .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart
                                          || o.Status == CulinaryCartConstants.Status.CheckedOut);

            if (order == null || !order.OrderItems.Any())
                return BadRequest("Cart is empty.");

            // ✅ FinalAmount is already seeded in DB by CartBAL
            var finalAmount = order.FinalAmount;
            var amountPaise = Convert.ToInt32(Math.Round(finalAmount * 100));

            var payload = new
            {
                amount = amountPaise,
                currency = "INR",
                receipt = $"rcpt_{Guid.NewGuid().ToString("N").Substring(0, 20)}",
                payment_capture = 1
            };

            var razorpayKey = _configuration["Razorpay:Key"];
            var razorpaySecret = _configuration["Razorpay:Secret"];
            using var client = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{razorpayKey}:{razorpaySecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response = await client.PostAsJsonAsync("https://api.razorpay.com/v1/orders", payload);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, $"Failed to create Razorpay order. Details: {responseContent}");

            var razorpayOrder = await response.Content.ReadFromJsonAsync<RazorpayOrderResponse>();

            // ✅ Return order details + Razorpay info
            return Ok(new
            {
                RazorpayOrderId = razorpayOrder.id,
                Amount = razorpayOrder.amount, // paise
                Currency = razorpayOrder.currency,
                FinalAmount = finalAmount,     // rupees
                Items = order.OrderItems.Select(i => new
                {
                    i.FoodItemId,
                    i.FoodItemName,
                    i.Quantity,
                    i.FinalPrice
                })
            });
        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayment([FromBody] PaymentVerificationDto dto)
        {
            var razorpaySecret = _configuration["Razorpay:Secret"];
            var payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(razorpaySecret));
            var hash = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)))
                .Replace("-", "").ToLower();

            var order = _orderHistoryDal.GetByRazorpayOrderId(dto.RazorpayOrderId);

            if (hash == dto.RazorpaySignature)
            {
                if (order != null)
                {
                    order.Status = "Success";
                    order.PaymentId = dto.RazorpayPaymentId;
                    _orderHistoryDal.Update(order);
                }
                return Ok(new { success = true, message = "Payment verified successfully." });
            }
            else
            {
                if (order != null)
                {
                    order.Status = "Failed";
                    _orderHistoryDal.Update(order);
                }
                return BadRequest(new { success = false, message = "Payment verification failed." });
            }
        }
    }
}

