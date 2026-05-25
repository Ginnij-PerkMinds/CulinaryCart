using CulinaryCart.DbContext;
using CulinaryCart.Model;

namespace CulinaryCart.CulinaryDal
{
    public class OrderHistoryDAL
    {
        private readonly CulinaryCartDbContext _db;
        public OrderHistoryDAL(CulinaryCartDbContext db) 
        {
            _db = db; 
        }

        public void Add(OrderHistory history) 
        { 
            _db.OrderHistory.Add(history); 
            _db.SaveChanges(); 
        }
        public void Update(OrderHistory history) 
        { 
            _db.OrderHistory.Update(history); 
            _db.SaveChanges();
        }
        public void Delete(OrderHistory history) 
        { 
            _db.OrderHistory.Remove(history); 
            _db.SaveChanges(); 
        }
        public List<OrderHistory> GetAll()
        {
            return _db.OrderHistory.ToList();
        }
    }
}
