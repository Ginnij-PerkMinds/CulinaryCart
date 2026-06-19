namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class Loginresponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
