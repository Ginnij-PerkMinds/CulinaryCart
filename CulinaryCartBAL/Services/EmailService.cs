using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) { _config = config; }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("CulinaryCart", _config["Email:From"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Your CulinaryCart OTP Code";
        message.Body = new TextPart("plain")
        {
            Text = $"Your OTP code is: {otp}. It expires in 10 minutes."
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_config["Email:SmtpServer"], int.Parse(_config["Email:Port"]), SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

