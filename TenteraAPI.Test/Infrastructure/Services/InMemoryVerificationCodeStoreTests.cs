using TenteraAPI.Domain.Interfaces.Services;
using TenteraAPI.Infrastructure.Services;
using Xunit;

namespace TenteraAPI.Test.Infrastructure.Services
{
    public class InMemoryVerificationCodeStoreTests
    {
        private readonly IVerificationCodeStore _codeStore;

        public InMemoryVerificationCodeStoreTests()
        {
            _codeStore = new InMemoryVerificationCodeStore();
        }

        [Fact]
        public async Task StoreCodeAsync_AndGetCodeAsync_ReturnsStoredCode()
        {
            // Arrange
            var key = "test@example.com";
            var code = "123456";
            var expiry = DateTime.UtcNow.AddMinutes(5);

            // Act
            await _codeStore.StoreCodeAsync(key, code, expiry);
            var result = await _codeStore.GetCodeAsync(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Value.Code);
            Assert.Equal(expiry, result.Value.Expiry);
        }

        [Fact]
        public async Task GetCodeAsync_WithNonExistentKey_ReturnsNull()
        {
            // Arrange
            var key = "nonexistent@example.com";

            // Act
            var result = await _codeStore.GetCodeAsync(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveCodeAsync_RemovesStoredCode()
        {
            // Arrange
            var key = "test@example.com";
            var code = "123456";
            var expiry = DateTime.UtcNow.AddMinutes(5);
            await _codeStore.StoreCodeAsync(key, code, expiry);

            // Act
            await _codeStore.RemoveCodeAsync(key);
            var result = await _codeStore.GetCodeAsync(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task StoreCodeAsync_OverwritesExistingCode()
        {
            // Arrange
            var key = "test@example.com";
            var oldCode = "123456";
            var oldExpiry = DateTime.UtcNow.AddMinutes(5);
            var newCode = "654321";
            var newExpiry = DateTime.UtcNow.AddMinutes(10);

            // Act
            await _codeStore.StoreCodeAsync(key, oldCode, oldExpiry);
            await _codeStore.StoreCodeAsync(key, newCode, newExpiry);
            var result = await _codeStore.GetCodeAsync(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newCode, result.Value.Code);
            Assert.Equal(newExpiry, result.Value.Expiry);
        }

        [Fact]
        public async Task GetCodeAsync_WithExpiredCode_ReturnsNull()
        {
            // Arrange
            var key = "test@example.com";
            var code = "123456";
            var expiry = DateTime.UtcNow.AddMinutes(-1); // Expired code

            // Act
            await _codeStore.StoreCodeAsync(key, code, expiry);
            var result = await _codeStore.GetCodeAsync(key);

            // Assert
            Assert.Null(result);
        }
    }
} 