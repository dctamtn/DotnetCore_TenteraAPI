namespace TenteraAPI.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string email, string code);
    }
}
