using TenteraAPI.Domain.Interfaces.Services;
using System.Net.Mail;

namespace TenteraAPI.Infrastructure.Services
{
    public interface IEmailClientWrapper
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class SmtpEmailClientWrapper : IEmailClientWrapper
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public SmtpEmailClientWrapper(string host, int port, string username, string password, string fromEmail)
        {
            _smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(username, password)
            };
            _fromEmail = fromEmail;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var message = new MailMessage(_fromEmail, to, subject, body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(message);
        }
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailClientWrapper _emailClient;

        public EmailService(IConfiguration configuration, IEmailClientWrapper emailClient = null)
        {
            var host = configuration["SmtpSettings:Host"] 
                ?? throw new ArgumentNullException("SmtpSettings:Host is not configured");
            var port = int.Parse(configuration["SmtpSettings:Port"] 
                ?? throw new ArgumentNullException("SmtpSettings:Port is not configured"));
            var username = configuration["SmtpSettings:Username"] 
                ?? throw new ArgumentNullException("SmtpSettings:Username is not configured");
            var password = configuration["SmtpSettings:Password"] 
                ?? throw new ArgumentNullException("SmtpSettings:Password is not configured");
            var fromEmail = configuration["SmtpSettings:FromEmail"] 
                ?? throw new ArgumentNullException("SmtpSettings:FromEmail is not configured");

            _emailClient = emailClient ?? new SmtpEmailClientWrapper(host, port, username, password, fromEmail);
        }

        public async Task SendVerificationCodeAsync(string email, string code)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Verification code is required");

            try
            {
                await _emailClient.SendEmailAsync(
                    email,
                    "Your Verification Code",
                    $"Your verification code is {code}. It expires in 10 minutes."
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}

