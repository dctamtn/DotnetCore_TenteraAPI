using Microsoft.Extensions.Configuration;
using Moq;
using TenteraAPI.Domain.Interfaces.Services;
using TenteraAPI.Infrastructure.Services;

namespace TenteraAPI.Test.Infrastructure.Services
{
    public class EmailServiceTests
    {
        private readonly IEmailService _emailService;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IEmailClientWrapper> _emailClientMock;

        public EmailServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["SmtpSettings:Host"]).Returns("smtp.example.com");
            _configurationMock.Setup(x => x["SmtpSettings:Port"]).Returns("587");
            _configurationMock.Setup(x => x["SmtpSettings:Username"]).Returns("test@example.com");
            _configurationMock.Setup(x => x["SmtpSettings:Password"]).Returns("test_password");
            _configurationMock.Setup(x => x["SmtpSettings:FromEmail"]).Returns("test@example.com");

            _emailClientMock = new Mock<IEmailClientWrapper>();
            _emailService = new EmailService(_configurationMock.Object, _emailClientMock.Object);
        }

        [Fact]
        public async Task SendVerificationCodeAsync_WithValidEmail_SendsEmail()
        {
            // Arrange
            var email = "test@example.com";
            var code = "123456";

            // Act
            await _emailService.SendVerificationCodeAsync(email, code);

            // Assert
            _emailClientMock.Verify(x => x.SendEmailAsync(
                email,
                "Your Verification Code",
                $"Your verification code is {code}. It expires in 10 minutes."
            ), Times.Once);
        }

        [Fact]
        public async Task SendVerificationCodeAsync_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var email = "invalid-email";
            var code = "123456";

            _emailClientMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Invalid email format"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _emailService.SendVerificationCodeAsync(email, code));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendVerificationCodeAsync_WithEmptyEmail_ThrowsException(string email)
        {
            // Arrange
            var code = "123456";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _emailService.SendVerificationCodeAsync(email, code));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendVerificationCodeAsync_WithEmptyCode_ThrowsException(string code)
        {
            // Arrange
            var email = "test@example.com";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _emailService.SendVerificationCodeAsync(email, code));
        }

        [Fact]
        public void Constructor_WithMissingConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["SmtpSettings:Host"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new EmailService(configuration.Object));
        }

        [Fact]
        public void Constructor_WithInvalidPort_ThrowsFormatException()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["SmtpSettings:Host"]).Returns("smtp.example.com");
            configuration.Setup(x => x["SmtpSettings:Port"]).Returns("invalid_port");
            configuration.Setup(x => x["SmtpSettings:Username"]).Returns("test@example.com");
            configuration.Setup(x => x["SmtpSettings:Password"]).Returns("test_password");
            configuration.Setup(x => x["SmtpSettings:FromEmail"]).Returns("test@example.com");

            // Act & Assert
            Assert.Throws<FormatException>(() => new EmailService(configuration.Object));
        }
    }
} 