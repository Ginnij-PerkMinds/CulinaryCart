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

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); 
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}

