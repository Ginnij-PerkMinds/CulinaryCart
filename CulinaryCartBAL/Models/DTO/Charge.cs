namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class ChargeDto
    {
        public int ChargeId { get; set; }
        public string ChargeType { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
    }

    public class AddChargeRequest
    {
        public string ChargeType { get; set; }
        public string Value { get; set; }   
        public bool IsActive { get; set; }
    }

    public class UpdateChargeRequest
    {
        public int ChargeId { get; set; }
        public string ChargeType { get; set; }
        public string Value { get; set; }   
        public bool IsActive { get; set; }
    }
}