using Microsoft.Extensions.Configuration;
using Moq;
using TenteraAPI.Domain.Interfaces.Services;
using TenteraAPI.Infrastructure.Services;
using Xunit;

namespace TenteraAPI.Test.Infrastructure.Services
{
    public class SmsServiceTests
    {
        private readonly ISmsService _smsService;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ITwilioClientWrapper> _twilioClientMock;

        public SmsServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["TwilioSettings:AccountSid"]).Returns("test_account_sid");
            _configurationMock.Setup(x => x["TwilioSettings:AuthToken"]).Returns("test_auth_token");
            _configurationMock.Setup(x => x["TwilioSettings:FromNumber"]).Returns("+1234567890");

            _twilioClientMock = new Mock<ITwilioClientWrapper>();
            _smsService = new SmsService(_configurationMock.Object, _twilioClientMock.Object);
        }

        [Fact]
        public async Task SendVerificationCodeAsync_WithValidPhoneNumber_SendsSms()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var code = "123456";

            // Act
            await _smsService.SendVerificationCodeAsync(phoneNumber, code);

            // Assert
            _twilioClientMock.Verify(x => x.SendMessageAsync(
                phoneNumber,
                "+1234567890",
                $"Your verification code is {code}. It expires in 10 minutes."
            ), Times.Once);
        }

        [Fact]
        public async Task SendVerificationCodeAsync_WithInvalidPhoneNumber_ThrowsException()
        {
            // Arrange
            var phoneNumber = "123456"; // Invalid format
            var code = "123456";

            _twilioClientMock.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Invalid phone number format"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _smsService.SendVerificationCodeAsync(phoneNumber, code));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendVerificationCodeAsync_WithEmptyPhoneNumber_ThrowsException(string phoneNumber)
        {
            // Arrange
            var code = "123456";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _smsService.SendVerificationCodeAsync(phoneNumber, code));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendVerificationCodeAsync_WithEmptyCode_ThrowsException(string code)
        {
            // Arrange
            var phoneNumber = "+1234567890";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _smsService.SendVerificationCodeAsync(phoneNumber, code));
        }

        [Fact]
        public void Constructor_WithMissingConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["TwilioSettings:AccountSid"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SmsService(configuration.Object));
        }
    }
} 