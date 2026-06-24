using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryFAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserBAL
{
    private readonly UserDAL _userDal;
    private readonly IImageFAL _imageFal;

    public UserBAL(UserDAL userDal, IImageFAL imageFal)
    {
        _userDal = userDal;
        _imageFal = imageFal;
    }

    public string Signup(SignupDto Dto)
    {
        // Check if email already exists
        var existingUser = _userDal.GetByEmail(Dto.Email);
        if (existingUser != null)
        {
            return "User already exists";
        }

        // Validate password
        if (!IsValidPassword(Dto.Password))
        {
            return "Password must be at least 8 characters, include uppercase, lowercase, digit, and special character.";
        }

        // Hash password 
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

        var result = _userDal.Add(newUser);
        return result;
    }

    private bool IsValidPassword(string password)
    {
        var hasUpper = new Regex(@"[A-Z]+");
        var hasLower = new Regex(@"[a-z]+");
        var hasDigit = new Regex(@"\d+");
        var hasSpecial = new Regex(@"[\W]+");
        return password.Length >= 8 &&
               hasUpper.IsMatch(password) &&
               hasLower.IsMatch(password) &&
               hasDigit.IsMatch(password) &&
               hasSpecial.IsMatch(password);
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

    public Loginresponse? Loginresponse(string email, string password)
    {
        var user = _userDal.GetByEmail(email);
        if (user == null) return null;

        string hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword) return null;

        var claims = new[]       // <-- added 24-06
       {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("EmailId", user.EmailId),
            new Claim("IsAdmin", user.IsAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Generate token (your existing logic)
        //var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{DateTime.Now}"));
         
        var token = new JwtSecurityToken(      // <-- added 24-06
            issuer: "your-app",
            audience: "your-app",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        // Return token + role info
        return new Loginresponse
        {
            //Token = token,
            //Email = user.EmailId,
            //IsAdmin = user.IsAdmin
            Token = new JwtSecurityTokenHandler().WriteToken(token),     // <-- added 24-06
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

            // Flattened address string
            Address = string.Join(", ",
            new[] {
                u.Address?.HouseNo,
                u.Address?.Locality,
                u.Address?.Landmark,
                u.Address?.City,
                u.Address?.District,
                u.Address?.Pincode,
                u.Address?.State
            }.Where(x => !string.IsNullOrWhiteSpace(x))
        )
        }).ToList();
    }

    public string UpdateFlags(int id, UpdateFlagsDto dto)
    {
        var user = _userDal.GetById(id);
        if (user == null) return "User not found";

        user.IsActive = dto.IsActive;
        user.IsAdmin = dto.IsAdmin;

        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        user.UpdatedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, istZone);

        var result = _userDal.UpdateUser(user);
        return result ? "Flags updated successfully" : "User not found";
    }
    public string UpdateUserProfile(int id, UpdateUserDto dto)
    {
        var user = _userDal.GetByIdWithAddress(id);
        if (user == null) return "User not found";


        if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.EmailId)) user.EmailId = dto.EmailId;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNo)) user.PhoneNo = dto.PhoneNo;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (!IsValidPassword(dto.Password))
                return "Password must be at least 8 characters, include uppercase, lowercase, digit, and special character.";
            user.PasswordHash = HashPassword(dto.Password);
        }

        if (dto.ProfilePic != null)
        {
            _imageFal.DeleteImage(user.ProfilePic);
            user.ProfilePic = _imageFal.SaveImage(dto.ProfilePic);
        }

        if (user.Address == null) user.Address = new Address();
        user.Address.HouseNo = dto.HouseNo ?? user.Address.HouseNo;
        user.Address.Locality = dto.Locality ?? user.Address.Locality;
        user.Address.Landmark = dto.Landmark ?? user.Address.Landmark;
        user.Address.City = dto.City ?? user.Address.City;
        user.Address.District = dto.District ?? user.Address.District;
        user.Address.Pincode = dto.Pincode ?? user.Address.Pincode;
        user.Address.State = dto.State ?? user.Address.State;



        user.UpdatedAt = DateTimeOffset.Now;

        var result = _userDal.UpdateUser(user);
        return result ? "User updated successfully" : "User not found";
    }

    public string ChangePassword(int id, string oldPassword, string newPassword)
    {
        var user = _userDal.GetById(id);
        if (user == null) return "User not found";

        string oldHash = HashPassword(oldPassword);
        if (user.PasswordHash != oldHash) return "Incorrect old password";

        if (!IsValidPassword(newPassword))
            return "Password must be at least 8 characters, include uppercase, lowercase, digit, and special character.";

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTimeOffset.Now;

        var result = _userDal.UpdateUser(user);
        return result ? "Password updated successfully" : "Error updating password";
    }



    public string DeleteUser(int id)
    {
        var user = _userDal.GetById(id);
        if (user == null) return "User not found";
        _userDal.Delete(user);
        return "User deleted successfully";
    }
}










