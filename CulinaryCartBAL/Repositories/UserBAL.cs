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

        var newUser = new User
        {
            Name = name,
            EmailId = email,
            PasswordHash = hashedPassword,
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

        // ✅ Normally generate JWT here
        // For demo, return a fake token
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{DateTime.Now}"));
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes("YourSuperSecretKey123!"); // store securely
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //        new Claim(ClaimTypes.Name, user.Name),
        //        new Claim(ClaimTypes.Email, user.EmailId)
        //    }),
        //        Expires = DateTime.UtcNow.AddMinutes(30),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(key),
        //            SecurityAlgorithms.HmacSha256Signature
        //        )
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
    }

}

