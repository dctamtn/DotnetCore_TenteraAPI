using Moq;
using TenteraAPI.Application.Services;
using TenteraAPI.Domain.Entities;
using TenteraAPI.Application.DTOs;
using TenteraAPI.Domain.Interfaces.Services;
using Xunit;
using TenteraAPI.Domain.Interfaces.Repositories;

namespace TenteraAPI.Test.Application.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ISmsService> _smsServiceMock;
        private readonly Mock<IVerificationCodeStore> _codeStoreMock;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _smsServiceMock = new Mock<ISmsService>();
            _codeStoreMock = new Mock<IVerificationCodeStore>();
            _accountService = new AccountService(
                _accountRepositoryMock.Object,
                _emailServiceMock.Object,
                _smsServiceMock.Object,
                _codeStoreMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new AccountRegistrationDto
            {
                CustomerName = "John Doe",
                ICNumber = "123456789",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                HasAcceptedPrivacyPolicy = true
            };

            _accountRepositoryMock.Setup(x => x.ICNumberExistsAsync(dto.ICNumber))
                .ReturnsAsync(false);
            _accountRepositoryMock.Setup(x => x.EmailExistsAsync(dto.Email))
                .ReturnsAsync(false);
            _accountRepositoryMock.Setup(x => x.PhoneNumberExistsAsync(dto.PhoneNumber))
                .ReturnsAsync(false);

            // Act
            var result = await _accountService.RegisterAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Customer registered successfully, verification codes sent", result.Message);
            Assert.NotNull(result.CustomerId);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingICNumber_ReturnsFailure()
        {
            // Arrange
            var dto = new AccountRegistrationDto
            {
                CustomerName = "John Doe",
                ICNumber = "123456789",
                Email = "john@example.com",
                PhoneNumber = "+1234567890"
            };

            _accountRepositoryMock.Setup(x => x.ICNumberExistsAsync(dto.ICNumber))
                .ReturnsAsync(true);

            // Act
            var result = await _accountService.RegisterAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("ICNumber already registered", result.Message);
            Assert.Null(result.CustomerId);
        }

        [Fact]
        public async Task SendEmailVerificationCodeAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new RequestSendCodeDto { ICNumber = "123456789" };
            var account = new Account { Email = "test@example.com" };

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.SendEmailVerificationCodeAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Email verification code sent", result.Message);
        }

        [Fact]
        public async Task SendMobileVerificationCodeAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new RequestSendCodeDto { ICNumber = "123456789" };
            var account = new Account { PhoneNumber = "+1234567890" };

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.SendMobileVerificationCodeAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Mobile verification code sent", result.Message);
        }

        [Fact]
        public async Task VerifyCodeAsync_WithValidCode_ReturnsSuccess()
        {
            // Arrange
            var dto = new VerifyCodeDto 
            { 
                ICNumber = "123456789",
                Code = "123456",
                Type = TypeVerify.EMAIL
            };
            var account = new Account 
            { 
                Email = "test@example.com",
                IsEmailVerified = false
            };
            var storedCode = ("123456", DateTime.UtcNow.AddMinutes(5));

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);
            _codeStoreMock.Setup(x => x.GetCodeAsync(account.Email))
                .ReturnsAsync(storedCode);
            _codeStoreMock.Setup(x => x.RemoveCodeAsync(account.Email))
                .Returns(Task.CompletedTask);
            _accountRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Account>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.VerifyCodeAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Code verified successfully", result.Message);
            _codeStoreMock.Verify(x => x.RemoveCodeAsync(account.Email), Times.Once);
            _accountRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Account>(a => 
                a.IsEmailVerified == true)), Times.Once);
        }

        [Fact]
        public async Task VerifyCodeAsync_WithInvalidCode_ReturnsFailure()
        {
            // Arrange
            var dto = new VerifyCodeDto 
            { 
                ICNumber = "123456789",
                Code = "wrongcode",
                Type = TypeVerify.EMAIL
            };
            var account = new Account { Email = "test@example.com" };
            var storedCode = ("123456", DateTime.UtcNow.AddMinutes(5));

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);
            _codeStoreMock.Setup(x => x.GetCodeAsync(account.Email))
                .ReturnsAsync(storedCode);

            // Act
            var result = await _accountService.VerifyCodeAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Incorrect code", result.Message);
        }

        [Fact]
        public async Task VerifyCodeAsync_WithExpiredCode_ReturnsFailure()
        {
            // Arrange
            var dto = new VerifyCodeDto 
            { 
                ICNumber = "123456789",
                Code = "123456",
                Type = TypeVerify.EMAIL
            };
            var account = new Account { Email = "test@example.com" };
            var storedCode = ("123456", DateTime.UtcNow.AddMinutes(-1)); // Expired code

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);
            _codeStoreMock.Setup(x => x.GetCodeAsync(account.Email))
                .ReturnsAsync(storedCode);

            // Act
            var result = await _accountService.VerifyCodeAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid or expired code", result.Message);
        }

        [Fact]
        public async Task VerifyCodeAsync_WithPhoneVerification_ReturnsSuccess()
        {
            // Arrange
            var dto = new VerifyCodeDto 
            { 
                ICNumber = "123456789",
                Code = "123456",
                Type = TypeVerify.PHONE
            };
            var account = new Account 
            { 
                PhoneNumber = "+1234567890",
                IsPhoneVerified = false
            };
            var storedCode = ("123456", DateTime.UtcNow.AddMinutes(5));

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);
            _codeStoreMock.Setup(x => x.GetCodeAsync(account.PhoneNumber))
                .ReturnsAsync(storedCode);
            _codeStoreMock.Setup(x => x.RemoveCodeAsync(account.PhoneNumber))
                .Returns(Task.CompletedTask);
            _accountRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Account>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.VerifyCodeAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Code verified successfully", result.Message);
            _codeStoreMock.Verify(x => x.RemoveCodeAsync(account.PhoneNumber), Times.Once);
            _accountRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Account>(a => 
                a.IsPhoneVerified == true)), Times.Once);
        }

        [Fact]
        public async Task CreatePin_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new PinRequestDto 
            { 
                ICNumber = "123456789",
                PinHash = "hashedPin"
            };
            var account = new Account 
            { 
                IsEmailVerified = true,
                IsPhoneVerified = true,
                HasAcceptedPrivacyPolicy = true
            };

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.CreatePin(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("PIN created successfully", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var dto = new LoginDto 
            { 
                ICNumber = "123456789",
                PinHash = "hashedPin",
                UseFaceBiometric = false,
                UseFingerprintBiometric = false
            };
            var account = new Account 
            { 
                Id = 1,
                PinHash = "hashedPin",
                IsEmailVerified = true,
                IsPhoneVerified = true,
                HasAcceptedPrivacyPolicy = true
            };

            _accountRepositoryMock.Setup(x => x.GetICNumberAsync(dto.ICNumber))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.LoginAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal(1, result.CustomerId);
        }
    }
} 