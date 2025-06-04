using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TenteraAPI.Application.DTOs;
using TenteraAPI.Domain.Interfaces.Services;

namespace TenteraAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new account
        /// </summary>
        /// <param name="dto">Account registration details including personal information</param>
        /// <returns>Success message and customer ID if registration is successful</returns>
        /// <response code="200">Returns the success message and customer ID</response>
        /// <response code="400">If the registration data is invalid or registration fails</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AccountRegistrationDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration data for email: {Email}", dto.Email);
                return BadRequest(ModelState);
            }

            var (success, message, customerId) = await _accountService.RegisterAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Registration successful for email: {Email}, CustomerId: {CustomerId}", dto.Email, customerId);
            }
            else
            {
                _logger.LogWarning("Registration failed for email: {Email}. Reason: {Message}", dto.Email, message);
            }

            return success
                ? Ok(new { message, customerId })
                : BadRequest(new { message });
        }

        /// <summary>
        /// Send email verification code
        /// </summary>
        /// <param name="dto">Email address to send verification code</param>
        /// <returns>Success message if code is sent successfully</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the email is invalid or sending fails</response>
        [HttpPost("send-email-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendEmailVerificationCode([FromBody] RequestSendCodeDto dto)
        {
            _logger.LogInformation("Email verification code request for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid email verification request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.SendEmailVerificationCodeAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Email verification code sent successfully for IC Number: {ICNumber}", dto.ICNumber);
            }
            else
            {
                _logger.LogWarning("Failed to send email verification code for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>
        /// Send mobile verification code
        /// </summary>
        /// <param name="dto">Phone number to send verification code</param>
        /// <returns>Success message if code is sent successfully</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the phone number is invalid or sending fails</response>
        [HttpPost("send-mobile-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMobileVerificationCode([FromBody] RequestSendCodeDto dto)
        {
            _logger.LogInformation("Mobile verification code request for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid mobile verification request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.SendMobileVerificationCodeAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Mobile verification code sent successfully for IC Number: {ICNumber}", dto.ICNumber);
            }
            else
            {
                _logger.LogWarning("Failed to send mobile verification code for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>
        /// Verify the code sent to email or mobile
        /// </summary>
        /// <param name="dto">Verification code and contact information</param>
        /// <returns>Success message if verification is successful</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the code is invalid or verification fails</response>
        [HttpPost("verify-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
        {
            _logger.LogInformation("Code verification attempt for IC Number: {ICNumber}, Type: {Type}", dto.ICNumber, dto.Type);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid code verification request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.VerifyCodeAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Code verified successfully for IC Number: {ICNumber}, Type: {Type}", dto.ICNumber, dto.Type);
            }
            else
            {
                _logger.LogWarning("Code verification failed for IC Number: {ICNumber}, Type: {Type}. Reason: {Message}", dto.ICNumber, dto.Type, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>
        /// Create or update PIN for the account
        /// </summary>
        /// <param name="dto">PIN details including the PIN and customer ID</param>
        /// <returns>Success message if PIN is created successfully</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the PIN is invalid or creation fails</response>
        [HttpPost("create-pin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePin([FromBody] PinRequestDto dto)
        {
            _logger.LogInformation("PIN creation attempt for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid PIN creation request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.CreatePin(dto);
            
            if (success)
            {
                _logger.LogInformation("PIN created successfully for IC Number: {ICNumber}", dto.ICNumber);
            }
            else
            {
                _logger.LogWarning("PIN creation failed for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>
        /// Login to the account using PIN or biometric
        /// </summary>
        /// <param name="dto">Login credentials including PIN or biometric options</param>
        /// <returns>Success message, customer ID, and biometric usage status if login is successful</returns>
        /// <response code="200">Returns the success message and customer details</response>
        /// <response code="400">If the login credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login attempt for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message, customerId) = await _accountService.LoginAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Login successful for IC Number: {ICNumber}, CustomerId: {CustomerId}", dto.ICNumber, customerId);
            }
            else
            {
                _logger.LogWarning("Login failed for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success
                ? Ok(new { message, customerId, faceBiometricUsed = dto.UseFaceBiometric, fingerprintBiometricUsed = dto.UseFingerprintBiometric })
                : BadRequest(new { message });
        }

        /// <summary>
        /// Enable or disable face biometric authentication
        /// </summary>
        /// <param name="dto">Biometric request details including customer ID and enable/disable flag</param>
        /// <returns>Success message if biometric setting is updated successfully</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the request is invalid or update fails</response>
        [HttpPost("biometric/face")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ManageFaceBiometric([FromBody] BiometricRequestDto dto)
        {
            _logger.LogInformation("Face biometric management attempt for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid face biometric request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.ManageFaceBiometricAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Face biometric updated successfully for IC Number: {ICNumber}", dto.ICNumber);
            }
            else
            {
                _logger.LogWarning("Face biometric update failed for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>
        /// Enable or disable fingerprint biometric authentication
        /// </summary>
        /// <param name="dto">Biometric request details including customer ID and enable/disable flag</param>
        /// <returns>Success message if biometric setting is updated successfully</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="400">If the request is invalid or update fails</response>
        [HttpPost("biometric/fingerprint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ManageFingerprintBiometric([FromBody] BiometricRequestDto dto)
        {
            _logger.LogInformation("Fingerprint biometric management attempt for IC Number: {ICNumber}", dto.ICNumber);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid fingerprint biometric request for IC Number: {ICNumber}", dto.ICNumber);
                return BadRequest(ModelState);
            }

            var (success, message) = await _accountService.ManageFingerprintBiometricAsync(dto);
            
            if (success)
            {
                _logger.LogInformation("Fingerprint biometric updated successfully for IC Number: {ICNumber}", dto.ICNumber);
            }
            else
            {
                _logger.LogWarning("Fingerprint biometric update failed for IC Number: {ICNumber}. Reason: {Message}", dto.ICNumber, message);
            }

            return success ? Ok(new { message }) : BadRequest(new { message });
        }
    }
}
