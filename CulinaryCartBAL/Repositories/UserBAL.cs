using CulinaryCart.CulinaryCartBAL.Constants;
using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryFAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserBAL
{
    private readonly UserDAL _userDal;
    private readonly IImageFAL _imageFal;
    private readonly IConfiguration _configuration;
    private readonly CulinaryCartDbContext _context;

    //Injected IConfiguration inside the constructor block
    public UserBAL(UserDAL userDal, IImageFAL imageFal, IConfiguration configuration, CulinaryCartDbContext context)
    {
        _userDal = userDal;
        _imageFal = imageFal;
        _configuration = configuration;
        _context = context;
    }

    public string Signup(SignupDto Dto)
    {
        var existingUser = _userDal.GetByEmail(Dto.Email);
        if (existingUser != null) return CulinaryCartConstants.Messages.UserAlreadyExists;

        if (!IsValidPassword(Dto.Password))
        {
            return CulinaryCartConstants.Messages.PasswordRequirements;
        }

        string hashedPassword = HashPassword(Dto.Password);
        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        var nowIst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, istZone);

        var newUser = new User
        {
            Name = Dto.Name,
            EmailId = Dto.Email,
            PasswordHash = hashedPassword,
            CreatedAt = nowIst,
            IsActive = true,
            IsAdmin = false,
        };

        return _userDal.Add(newUser);
    }

    private bool IsValidPassword(string password)
    {
        var hasUpper = new Regex(@"[A-Z]+");
        var hasLower = new Regex(@"[a-z]+");
        var hasDigit = new Regex(@"\d+");
        var hasSpecial = new Regex(@"[\W]+");
        return password.Length >= 8 && hasUpper.IsMatch(password) && hasLower.IsMatch(password) && hasDigit.IsMatch(password) && hasSpecial.IsMatch(password);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    //Generates token using dynamic appsettings parameters safely
    public Loginresponse? Loginresponse(string email, string password)
    {
        var user = _userDal.GetByEmail(email);
        if (user == null) return null;

        string hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword) return null;

        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.EmailId),
            new Claim("IsAdmin", user.IsAdmin.ToString())
        };

        //Now mapping parameters directly from your secure AppSettings file!
        var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Secret Key missing from configurations.");
        var issuer = _configuration["Jwt:Issuer"] ?? "CulinaryCartAPI";
        var audience = _configuration["Jwt:Audience"] ?? "CulinaryCartAPI";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new Loginresponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.UserId,
            Name = user.Name,
            Email = user.EmailId,
            PhoneNo = user.PhoneNo,
            ProfilePic = user.ProfilePic,
            IsAdmin = user.IsAdmin
        };
    }


    public List<UserDto> GetAllUsers()
    {
        var users = _userDal.GetAllWithAddress();
        return users.Select(u => new UserDto
        {
            UserId = u.UserId,
            Name = u.Name,
            EmailId = u.EmailId,
            PhoneNo = u.PhoneNo,
            ProfilePic = u.ProfilePic,
            IsActive = u.IsActive,
            IsAdmin = u.IsAdmin,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            Address = string.Join(", ", new[] { u.Address?.HouseNo, u.Address?.Locality, u.Address?.Landmark, u.Address?.City, u.Address?.District, u.Address?.Pincode, u.Address?.State }.Where(x => !string.IsNullOrWhiteSpace(x))),

            HouseNo = u.Address?.HouseNo,
            Locality = u.Address?.Locality,
            Landmark = u.Address?.Landmark,
            City = u.Address?.City,
            District = u.Address?.District,
            Pincode = u.Address?.Pincode,
            State = u.Address?.State
        }).ToList();

    }

    public string UpdateFlags(int id, UpdateFlagsDto dto)
    {
        var user = _userDal.GetById(id);
        if (user == null) return CulinaryCartConstants.Messages.UserNotFound;

        user.IsActive = dto.IsActive;
        user.IsAdmin = dto.IsAdmin;

        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        user.UpdatedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, istZone);

        return _userDal.UpdateUser(user) ? CulinaryCartConstants.Messages.FlagsUpdated : CulinaryCartConstants.Messages.UserNotFound;
    }

    public string UpdateUserProfile(int id, UpdateUserDto dto)
    {
        var user = _userDal.GetByIdWithAddress(id);
        if (user == null) return CulinaryCartConstants.Messages.UserNotFound;

        if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.EmailId)) user.EmailId = dto.EmailId;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNo)) user.PhoneNo = dto.PhoneNo;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (!IsValidPassword(dto.Password)) return CulinaryCartConstants.Messages.PasswordUpdateFailed;
            user.PasswordHash = HashPassword(dto.Password);
        }

        if (dto.ProfilePic != null)
        {
            _imageFal.DeleteImage(user.ProfilePic);
            user.ProfilePic = _imageFal.SaveImage(dto.ProfilePic);
        }

        if (!string.IsNullOrWhiteSpace(dto.HouseNo)) user.Address.HouseNo = dto.HouseNo;
        if (!string.IsNullOrWhiteSpace(dto.Locality)) user.Address.Locality = dto.Locality;
        if (!string.IsNullOrWhiteSpace(dto.Landmark)) user.Address.Landmark = dto.Landmark;
        if (!string.IsNullOrWhiteSpace(dto.City)) user.Address.City = dto.City;
        if (!string.IsNullOrWhiteSpace(dto.District)) user.Address.District = dto.District;
        if (!string.IsNullOrWhiteSpace(dto.Pincode)) user.Address.Pincode = dto.Pincode;
        if (!string.IsNullOrWhiteSpace(dto.State)) user.Address.State = dto.State;

        user.UpdatedAt = DateTimeOffset.Now;
        return _userDal.UpdateUser(user) ? CulinaryCartConstants.Messages.UserUpdated : CulinaryCartConstants.Messages.UserNotFound;
    }

    public string ChangePassword(int id, string oldPassword, string newPassword)
    {
        var user = _userDal.GetById(id);
        if (user == null) return CulinaryCartConstants.Messages.UserNotFound;

        if (user.PasswordHash != HashPassword(oldPassword)) return CulinaryCartConstants.Messages.IncorrectOldPassword;
        if (!IsValidPassword(newPassword)) return CulinaryCartConstants.Messages.PasswordUpdateFailed;

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTimeOffset.Now;

        return _userDal.UpdateUser(user) ? CulinaryCartConstants.Messages.PasswordUpdateSuccessful : CulinaryCartConstants.Messages.PasswordUpdateFailed;
    }

    public string DeleteUser(int id)
    {
        var user = _userDal.GetById(id);
        if (user == null) return CulinaryCartConstants.Messages.UserNotFound;
        _userDal.Delete(user);
        return CulinaryCartConstants.Messages.UserDeleted;
    }
    public UserStatsDto GetUserStats()
    {
        return new UserStatsDto
        {
            TotalUsers = _context.Users.Count(),
            AdminUsers = _context.Users.Count(u => u.IsAdmin),
            ActiveUsers = _context.Users.Count(u => u.IsActive),
            TotalCustomers = _context.Users.Count(u => !u.IsAdmin),
            ActiveCustomers = _context.Users.Count(u => !u.IsAdmin && u.IsActive)
        };
    }

    public string Logout(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return CulinaryCartConstants.Messages.InvalidToken;

        var revoked = _userDal.RevokeToken(token);
        return revoked ? CulinaryCartConstants.Messages.LogoutSuccessful : CulinaryCartConstants.Messages.LogoutFailed;
    }

}