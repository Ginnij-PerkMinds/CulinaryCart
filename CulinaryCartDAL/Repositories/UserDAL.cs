using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;
using CulinaryCart.CulinaryCartDAL.DbContext;  


namespace CulinaryCart.CulinaryCartDAL.Repositories
{
   
    public class UserDAL
    {
        private readonly CulinaryCartDbContext _context;

        public UserDAL(CulinaryCartDbContext context)
        {
            _context = context;
        }

        
        public User GetByEmail(string email)
        {
            
            return _context.Users
                .FirstOrDefault(u => u.EmailId.ToLower() == email.ToLower());
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        // ✅ Get user by Id (with address)
        public User GetByIdWithAddress(int id)
        {
            return _context.Users
                .Include(u => u.Address)
                .FirstOrDefault(u => u.UserId == id);
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        // ✅ Get all users (with address)
        public List<User> GetAllWithAddress()
        {
            return _context.Users
                .Include(u => u.Address)
                .ToList();
        }

        public string Add(User user)
        {
            if (user == null) return "User object cannot be null";
            try
            {
                _context.Users.Add(user);
                int result = _context.SaveChanges();

                return result > 0 ? "User saved successfully" : "No changes were saved";
            }

            catch (DbUpdateException ex)
            {
                Console.WriteLine("EF Error: " + (ex.InnerException?.Message ?? ex.Message));
                return $"Database error: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                return $"Unexpected error: {ex.Message}";
            }
        }


        public void Update(User user)
        {
            _context.Users.Update(user);

            if (user.Address != null)
            {
                _context.Entry(user.Address).State =
                    user.Address.AddressId == 0 ? EntityState.Added : EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(User user)
        { 
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}

