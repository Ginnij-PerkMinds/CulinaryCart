using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Models;

namespace CulinaryCart.CulinaryCartDAL.Repositories
{
    public class OtpDAL
    {
        private readonly CulinaryCartDbContext _db;
        public OtpDAL(CulinaryCartDbContext db) { _db = db; }

        public void AddOtp(OtpEntry entry)
        {
            _db.OtpStore.Add(entry);
            _db.SaveChanges();
        }
        public void DeleteOtp(OtpEntry entry)
        {
            _db.OtpStore.Remove(entry);
            _db.SaveChanges();
        }

        public OtpEntry? GetOtp(string email, string code)
        {
            return _db.OtpStore.FirstOrDefault(o => o.EmailId == email && o.Code == code);
        }
    }

}
