namespace TenteraAPI.Domain.Interfaces.Services
{
    public interface IVerificationCodeStore
    {
        Task StoreCodeAsync(string key, string code, DateTime expiry);
        Task<(string Code, DateTime Expiry)?> GetCodeAsync(string key);
        Task RemoveCodeAsync(string key);
    }
}
