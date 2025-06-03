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
            _codes.TryGetValue(key, out var code);
            return code;
            await Task.CompletedTask;
        }

        public async Task RemoveCodeAsync(string key)
        {
            _codes.TryRemove(key, out _);
            await Task.CompletedTask;
        }
    }
}
