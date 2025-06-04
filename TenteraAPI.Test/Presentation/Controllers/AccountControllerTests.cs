using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TenteraAPI.Application.DTOs;
using TenteraAPI.Domain.Interfaces.Services;
using TenteraAPI.Presentation.Controllers;
using System.Text.Json;

namespace TenteraAPI.Test.Presentation.Controllers
{
	public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _loggerMock = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_accountServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new AccountRegistrationDto();
            var expectedResult = (true, "Registration successful", 1);
            _accountServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<RegisterResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
            Assert.Equal(expectedResult.Item3, returnValue.customerId);
        }

        [Fact]
        public async Task Register_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AccountRegistrationDto();
            var expectedResult = (false, "Registration failed", 0);
            _accountServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonSerializer.Serialize(badRequestResult.Value);
            var returnValue = JsonSerializer.Deserialize<ErrorResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task SendEmailVerificationCode_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new RequestSendCodeDto();
            var expectedResult = (true, "Verification code sent successfully");
            _accountServiceMock.Setup(x => x.SendEmailVerificationCodeAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SendEmailVerificationCode(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task SendMobileVerificationCode_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new RequestSendCodeDto();
            var expectedResult = (true, "Verification code sent successfully");
            _accountServiceMock.Setup(x => x.SendMobileVerificationCodeAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SendMobileVerificationCode(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task VerifyCode_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new VerifyCodeDto();
            var expectedResult = (true, "Code verified successfully");
            _accountServiceMock.Setup(x => x.VerifyCodeAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.VerifyCode(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task CreatePin_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new PinRequestDto();
            var expectedResult = (true, "PIN created successfully");
            _accountServiceMock.Setup(x => x.CreatePin(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreatePin(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task Login_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new LoginDto 
            { 
                UseFaceBiometric = true,
                UseFingerprintBiometric = false
            };
            var expectedResult = (true, "Login successful", 1);
            _accountServiceMock.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<LoginResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
            Assert.Equal(expectedResult.Item3, returnValue.customerId);
            Assert.True(returnValue.faceBiometricUsed);
            Assert.False(returnValue.fingerprintBiometricUsed);
        }

        [Fact]
        public async Task ManageFaceBiometric_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new BiometricRequestDto();
            var expectedResult = (true, "Face biometric updated successfully");
            _accountServiceMock.Setup(x => x.ManageFaceBiometricAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.ManageFaceBiometric(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task ManageFingerprintBiometric_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new BiometricRequestDto();
            var expectedResult = (true, "Fingerprint biometric updated successfully");
            _accountServiceMock.Setup(x => x.ManageFingerprintBiometricAsync(dto))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.ManageFingerprintBiometric(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var returnValue = JsonSerializer.Deserialize<MessageResponse>(json);
            Assert.Equal(expectedResult.Item2, returnValue.message);
        }

        [Fact]
        public async Task Register_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AccountRegistrationDto();
            _controller.ModelState.AddModelError("Property", "Error message");

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dto = new LoginDto();
            _controller.ModelState.AddModelError("Property", "Error message");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // Response models for deserialization
        private class MessageResponse
        {
            public string message { get; set; }
        }

        private class ErrorResponse
        {
            public string message { get; set; }
        }

        private class RegisterResponse
        {
            public string message { get; set; }
            public int? customerId { get; set; }
        }

        private class LoginResponse
        {
            public string message { get; set; }
            public int? customerId { get; set; }
            public bool faceBiometricUsed { get; set; }
            public bool fingerprintBiometricUsed { get; set; }
        }
    }
}