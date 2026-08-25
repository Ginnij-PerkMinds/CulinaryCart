using System.Text.Json.Serialization;

namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class ForgotPasswordDto
    {
        [JsonPropertyName("email")]
        public string EmailId { get; set; }
    }

    public class VerifyOtpDto
    {
        [JsonPropertyName("email")]
        public string EmailId { get; set; }
        public string Code { get; set; }
    }

    public class ResetPasswordDto
    {
        [JsonPropertyName("email")]
        public string EmailId { get; set; }
        public string NewPassword { get; set; }
    }

}
