using CulinaryCart.DbContext;
using CulinaryCart.Model;
using System.Collections.Generic;
using System.Linq;

namespace CulinaryCart.CulinaryDal
{
    public class CartDAL
    {
        private readonly CulinaryCartDbContext _context;

        public CartDAL(CulinaryCartDbContext context)
        {
            _context = context;
        }

        // Get all cart items
        public IEnumerable<CartItem> GetAll()
        {
            return _context.CartItems.ToList();
        }

        // Add new item to cart 
        public void Add(CartItem item)
        {
            _context.CartItems.Add(item);
            _context.SaveChanges();
        }

        //Update cart item
        public void Update(CartItem item)
        {
            _context.CartItems.Update(item);
            _context.SaveChanges();
        }

        //Delete cart item
        public void Delete(CartItem item)
        {
            _context.CartItems.Remove(item);
            _context.SaveChanges();
        }

        // Clearing cart after checkout
        public void Clear()
        {
            _context.CartItems.RemoveRange(_context.CartItems);
            _context.SaveChanges();
        }
    }
}
