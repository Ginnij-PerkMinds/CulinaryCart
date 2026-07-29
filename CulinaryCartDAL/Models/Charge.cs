namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Charge
    {
        public int ChargeId { get; set; }       
        public string ChargeType { get; set; }  
        public decimal Value { get; set; }      
        public bool IsActive { get; set; }     
    }
}


