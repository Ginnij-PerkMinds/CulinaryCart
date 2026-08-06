using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Models.DTO.CulinaryCart.CulinaryCartBAL.DTOs;
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
        private readonly CartBAL _cartBal;

        public PaymentController(OrderHistoryDAL orderHistoryDal, IConfiguration configuration, CartBAL cartBal)
        {
            _orderHistoryDal = orderHistoryDal;
            _configuration = configuration;
            _cartBal = cartBal;
        }
           

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("UserId claim missing in token.");
            return int.Parse(userIdClaim.Value);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = GetUserIdFromToken();

            // Fetch active InCart order
            var order = _orderHistoryDal.GetByUser(userId)
                        .OrderByDescending(o => o.OrderDate)
                        .FirstOrDefault(o => o.Status == CulinaryCartConstants.Status.InCart);

            if (order == null || !order.OrderItems.Any())
                return BadRequest("Cart is empty.");

            // Reload from DB to avoid stale entity
            order = _orderHistoryDal.GetById(order.OrderId);

            Console.WriteLine($"[DB Check Before Razorpay] OrderId={order.OrderId}, FinalAmount={order.FinalAmount}, Promo={order.AppliedPromoCode}, Discount={order.PromoDiscount}");

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

            // Persist RazorpayOrderId in DB
            order.RazorpayOrderId = razorpayOrder.id;
            _orderHistoryDal.Update(order);

            // Build typed response
            var responseDto = new PaymentOrderResponseDto
            {
                RazorpayOrderId = razorpayOrder.id,
                Amount = razorpayOrder.amount, // paise
                Dto = new PaymentRequestDto
                {
                    BaseAmount = order.BaseAmount,
                    PromoDiscount = order.PromoDiscount,
                    AppliedPromoCode = order.AppliedPromoCode,
                    HandlingFee = order.HandlingFee,
                    DeliveryFee = order.DeliveryFee,
                    TaxAmount = order.TaxAmount,
                    FinalAmount = finalAmount,
                    Currency = razorpayOrder.currency
                },
                Items = order.OrderItems.Select(i => new CartItemDto
                {
                    FoodItemId = i.FoodItemId,
                    FoodItemName = i.FoodItemName,
                    Quantity = i.Quantity,
                    FinalPrice = i.FinalPrice
                }).ToList()
            };

            return Ok(responseDto);
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

        [HttpPost("finalize-checkout")]
        public IActionResult FinalizeCheckout()
        {
            int userId = GetUserIdFromToken();
            var order = _cartBal.Checkout(userId);
            if (order == null)
                return BadRequest("No active order to finalize.");

            return Ok(new { success = true, message = "Order finalized and stock updated." });
        }
    }
}