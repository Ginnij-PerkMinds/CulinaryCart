namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class Loginresponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string ProfilePic { get; set; }
    }
}
