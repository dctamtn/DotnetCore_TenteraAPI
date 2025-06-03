namespace TenteraAPI.Domain.Interfaces.Services
{
    public interface ISmsService
    {
        Task SendVerificationCodeAsync(string phoneNumber, string code);
    }
}
