using System.Collections.Concurrent;
using TenteraAPI.Domain.Interfaces.Services;

namespace TenteraAPI.Infrastructure.Services
{
    public class InMemoryVerificationCodeStore : IVerificationCodeStore
    {
        private static readonly ConcurrentDictionary<string, (string Code, DateTime Expiry)> _codes = new();

        public async Task StoreCodeAsync(string key, string code, DateTime expiry)
        {
            _codes[key] = (code, expiry);
            await Task.CompletedTask;
        }

        public async Task<(string Code, DateTime Expiry)?> GetCodeAsync(string key)
        {
            if (_codes.TryGetValue(key, out var code))
            {
                // Check if the code has expired
                if (code.Expiry > DateTime.UtcNow)
                {
                    return code;
                }
                // Remove expired code
                _codes.TryRemove(key, out _);
            }
            return null;
        }

        public async Task RemoveCodeAsync(string key)
        {
            _codes.TryRemove(key, out _);
            await Task.CompletedTask;
        }
    }
}
