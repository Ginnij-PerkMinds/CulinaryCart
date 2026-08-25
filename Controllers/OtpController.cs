using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly OtpBAL _otpBAL;
    private readonly EmailService _emailService;
    private readonly CulinaryCartDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserBAL _userBAL;

    public AuthController(OtpBAL otpBAL, EmailService emailService, CulinaryCartDbContext db, IPasswordHasher<User> passwordHasher, UserBAL userBAL)
    {
        _otpBAL = otpBAL;
        _emailService = emailService;
        _db = db;
        _passwordHasher = passwordHasher;
        _userBAL = userBAL;
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.EmailId == dto.EmailId);
        if (user == null) return BadRequest(new { message = "Email not registered" });

        var otp = new Random().Next(100000, 999999).ToString();
        _otpBAL.GenerateOtp(dto.EmailId, otp);
        await _emailService.SendOtpAsync(dto.EmailId, otp);

        return Ok(new { message = "OTP sent to email" });
    }

    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        if (!_otpBAL.VerifyOtp(dto.EmailId, dto.Code))
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified" });
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = _userBAL.ResetPassword(dto.EmailId, dto.NewPassword);

        if (result == CulinaryCartConstants.Messages.UserNotFound ||
            result == CulinaryCartConstants.Messages.PasswordRequirements ||
            result == CulinaryCartConstants.Messages.PasswordUpdateFailed)
        {
            return BadRequest(new { message = result });
        }

        return Ok(new { message = result });
    }

}

