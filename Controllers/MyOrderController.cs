using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MyOrdersController : ControllerBase
{
    private readonly MyOrdersBAL _bal;
    private readonly OrderHistoryDAL _orderHistoryDal;
    private readonly OrdersBAL _orders;

    public MyOrdersController(MyOrdersBAL bal, OrderHistoryDAL orderHistoryDal, OrdersBAL orders)
    {
        _bal = bal;
        _orderHistoryDal = orderHistoryDal;
        _orders = orders;
    }
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("UserId claim not found in token");
        return int.Parse(userIdClaim.Value);
    }

    [HttpGet("all")]
    public IActionResult GetMyOrders()
    {
        var userId = GetUserIdFromToken();
        var orders = _bal.GetUserOrders(userId);
        if (!orders.Any())
            return Ok(new { Message = "No past orders found." });
        return Ok(orders);
    }

    [HttpGet("details/{id}")]
    public IActionResult GetMyOrderDetails(int id)
    {
        var userId = GetUserIdFromToken();
        var order = _bal.GetUserOrderDetails(userId, id);
        if (order == null)
            return NotFound(new { Message = "Order not found." });
        return Ok(order);
    }

    [HttpGet("my-orders-debug")]
    public IActionResult GetMyOrdersDebug()
    {
        int userId = GetUserIdFromToken();
        Console.WriteLine($"[DEBUG] Extracted UserId from token: {userId}");

        var orders = _orderHistoryDal.GetCompletedOrdersByUser(userId);
        Console.WriteLine($"[DEBUG] Orders found for UserId={userId}: {orders.Count}");

        if (!orders.Any())
        {
            return Ok(new { Message = $"No past orders found for user {userId}. Check token claims and DB rows." });
        }

        return Ok(orders.Select(o => new
        {
            o.OrderId,
            o.OrderDate,
            o.Status,
            o.OrderStatus,
            o.FinalAmount,
            o.AppliedPromoCode
        }));
    }

    [HttpGet("recent")]
    public IActionResult GetRecentOrders()
    {
        var userId = GetUserIdFromToken();
        var cutoff = DateTime.UtcNow.AddMinutes(-60);

        var orders = _bal.GetUserOrders(userId)
                         .Where(o => o.OrderDate >= cutoff)
                         .ToList();

        if (!orders.Any())
            return Ok(new { Message = "No recent orders found within 60 minutes." });

        return Ok(orders);
    }

    [HttpGet("delivered/eligible")]
    public IActionResult GetDeliveredEligibleOrders()
    {
        var userId = GetUserIdFromToken();
        var cutoff = DateTime.Now.AddMinutes(-120);

        var orders = _orders.GetOrdersByStatus("Delivered")
                         .Where(o => o.OrderDate <= cutoff)
                         .ToList();

        return Ok(orders);
    }

}