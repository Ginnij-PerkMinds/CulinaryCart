using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserBAL
{
    private readonly UserDAL _userDal;

    public UserBAL(UserDAL userDal)
    {
        _userDal = userDal;
    }

    public string Signup(string name, string email, string password)
    {
        // Check if email already exists
        var existingUser = _userDal.GetByEmail(email);
        if (existingUser != null)
        {
            return "User already exists";
        }

        // Validate password
        if (!IsValidPassword(password))
        {
            return "Password must be at least 8 characters, include uppercase, lowercase, digit, and special character.";
        }

        // Hash password 
        string hashedPassword = HashPassword(password);

        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        var nowIst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, istZone);

        var newUser = new User
        {
            Name = name,
            EmailId = email,
            PasswordHash = hashedPassword,
            CreatedAt = nowIst,
            //UpdatedAt = nowIst,
            IsActive = true,
            IsAdmin = false,
        };

        _userDal.Add(newUser);
        return "User registered successfully";
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

    public string? Login(string email, string password)
    {
        var user = _userDal.GetByEmail(email);
        if (user == null) return null;

        string hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword) return null;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{DateTime.Now}"));
    }
       
        public List<UserDto> GetAllUsers()
        {
        var users = _userDal.GetAll();
        return users.Select(u => new UserDto
        {
            UserId = u.UserId,
            Name = u.Name,
            EmailId = u.EmailId,
            IsActive = u.IsActive,
            IsAdmin = u.IsAdmin,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
       }

    public string UpdateUser(int id, UpdateUserDto dto)
    {
        var user = _userDal.GetById(id);
        if (user == null) return "User not found";

        user.Name = dto.Name;
        user.EmailId = dto.EmailId;
        user.IsActive = dto.IsActive;
        user.IsAdmin = dto.IsAdmin;

        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        user.UpdatedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, istZone);

        _userDal.Update(user);
        return "User updated successfully";
    }

    public string DeleteUser(int id)
    {
        var user = _userDal.GetById(id);
        if (user == null) return "User not found";

        //user.IsActive = false;

        //_userDal.Update(user);
        _userDal.Delete(user);
        return "User deleted successfully";
    }
}



