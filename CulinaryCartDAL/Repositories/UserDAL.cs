using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class UserDAL
    {
        private readonly CulinaryCartDbContext _context;

        public UserDAL(CulinaryCartDbContext context)
        {
            _context = context;
        }

        public User GetByEmail(string email) =>
            _context.Users.FirstOrDefault(u => u.EmailId.ToLower() == email.ToLower());

        public User GetById(int id) =>
            _context.Users.FirstOrDefault(u => u.UserId == id);

        public User GetByIdWithAddress(int id) =>
            _context.Users.Include(u => u.Address).FirstOrDefault(u => u.UserId == id);

        public List<User> GetAllWithAddress() =>
            _context.Users.Include(u => u.Address).ToList();

        public string Add(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0 ? "User saved successfully" : "No changes were saved";
        }

        // update
        public bool UpdateUser(User user)
        {
            var existingUser = _context.Users.Include(u => u.Address).FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null) return false;

            existingUser.IsActive = user.IsActive;
            existingUser.IsAdmin = user.IsAdmin;

            existingUser.Name = user.Name ?? existingUser.Name;
            existingUser.EmailId = user.EmailId ?? existingUser.EmailId;
            existingUser.PhoneNo = user.PhoneNo ?? existingUser.PhoneNo;
            existingUser.PasswordHash = user.PasswordHash ?? existingUser.PasswordHash;
            existingUser.ProfilePic = user.ProfilePic ?? existingUser.ProfilePic;

            if (user.Address != null)
            {
                if (existingUser.Address == null) existingUser.Address = new Address();
                existingUser.Address.HouseNo = user.Address.HouseNo ?? existingUser.Address.HouseNo;
                existingUser.Address.Locality = user.Address.Locality ?? existingUser.Address.Locality;
                existingUser.Address.Landmark = user.Address.Landmark ?? existingUser.Address.Landmark;
                existingUser.Address.City = user.Address.City ?? existingUser.Address.City;
                existingUser.Address.District = user.Address.District ?? existingUser.Address.District;
                existingUser.Address.Pincode = user.Address.Pincode ?? existingUser.Address.Pincode;
                existingUser.Address.State = user.Address.State ?? existingUser.Address.State;
            }

            existingUser.UpdatedAt = DateTimeOffset.Now;
            _context.SaveChanges();
            return true;
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}







