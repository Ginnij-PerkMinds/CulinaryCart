using CulinaryCart.DbContext;
using CulinaryCart.Model;
using System.Collections.Generic;   
using System.Linq;

namespace CulinaryCart.CulinaryDal
{
    public class MenuDAL
    {
        private readonly CulinaryCartDbContext _db;
        public MenuDAL(CulinaryCartDbContext db) 
        { 
            _db = db; 
        }

        public List<Menu> GetAllMenuItems()
        {
            return _db.Menu.ToList();
        }
        public Menu? GetItem(int id)
        {
            //return _db.Menu.FirstOrDefault(m => m.FoodItemID == id);
            var item = _db.Menu.FirstOrDefault(m => m.FoodItemID == id);
            if (item == null)
                throw new InvalidOperationException($"Menu item with ID {id} not found.");
            return item;
        }
    }
}
