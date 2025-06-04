using TenteraAPI.Application.DTOs;

namespace TenteraAPI.Domain.Interfaces.Services
{
    public interface IAccountService
    {
        Task<(bool Success, string Message, int? CustomerId)> RegisterAsync(AccountRegistrationDto dto);
        Task<(bool Success, string Message)> SendEmailVerificationCodeAsync(RequestSendCodeDto info);
        Task<(bool Success, string Message)> SendMobileVerificationCodeAsync(RequestSendCodeDto info);
        Task<(bool Success, string Message)> VerifyCodeAsync(VerifyCodeDto dto);
        Task<(bool Success, string Message)> CreatePin(PinRequestDto dto);
        Task<(bool Success, string Message, int? CustomerId)> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message)> ManageFaceBiometricAsync(BiometricRequestDto dto);
        Task<(bool Success, string Message)> ManageFingerprintBiometricAsync(BiometricRequestDto dto);
    }
} 