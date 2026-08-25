using CulinaryCart.CulinaryCartDAL.Models;
using CulinaryCart.CulinaryCartDAL.Repositories;

namespace CulinaryCart.CulinaryCartBAL.Repositories
{
    public class OtpBAL
    {
        private readonly OtpDAL _dal;
        public OtpBAL(OtpDAL dal) { _dal = dal; }

        public void GenerateOtp(string emailId, string code)
        {
            var entry = new OtpEntry 
            {
                EmailId = emailId, 
                Code = code, 
                Expiry = DateTime.UtcNow.AddMinutes(10) 
            };

            _dal.AddOtp(entry);
        }

        public bool VerifyOtp(string email, string code)
        {
            var entry = _dal.GetOtp(email, code);
            if ( entry != null && entry.Expiry > DateTime.UtcNow)
            {
                _dal.DeleteOtp(entry);
                return true;
            }
            return false;
        }     
    }
}