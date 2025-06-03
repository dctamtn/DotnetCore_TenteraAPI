using System.Net;
using System.Net.Mail;
using TenteraAPI.Domain.Interfaces.Services;

namespace TenteraAPI.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendVerificationCodeAsync(string email, string code)
        {
            try
            {
                using var client = new SmtpClient("smtp.example.com", 587)
                {
                    Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                    EnableSsl = true
                };
                await client.SendMailAsync(new MailMessage
                {
                    From = new MailAddress("your-email@example.com"),
                    To = { email },
                    Subject = "Verification Code",
                    Body = $"Your verification code is {code}. It expires in 10 minutes."
                });
            }
            catch
            {
                throw new System.Exception("Failed to send email");
            }
        }
    }
}
