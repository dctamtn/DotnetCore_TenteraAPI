using Microsoft.AspNetCore.Mvc;
using TenteraAPI.Application.DTOs;
using TenteraAPI.Application.Services;

namespace TenteraAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService customerService)
        {
            _accountService = customerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegistrationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, customerId) = await _accountService.RegisterAsync(dto);
            return success
                ? Ok(new { message, customerId })
                : BadRequest(new { message });
        }

        [HttpPost("send-email-code")]
        public async Task<IActionResult> SendEmailVerificationCode([FromBody] RequestSendCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.SendEmailVerificationCodeAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("send-mobile-code")]
        public async Task<IActionResult> SendMobileVerificationCode([FromBody] RequestSendCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.SendMobileVerificationCodeAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.VerifyCodeAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("create-pin")]
        public async Task<IActionResult> CreatePin([FromBody] PinRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.CreatePin(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, customerId) = await _accountService.LoginAsync(dto);
            return success
                ? Ok(new { message, customerId, faceBiometricUsed = dto.UseFaceBiometric, fingerprintBiometricUsed = dto.UseFingerprintBiometric })
                : BadRequest(new { message });
        }

        [HttpPost("biometric/face")]
        public async Task<IActionResult> ManageFaceBiometric([FromBody] BiometricRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.ManageFaceBiometricAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("biometric/fingerprint")]
        public async Task<IActionResult> ManageFingerprintBiometric([FromBody] BiometricRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _accountService.ManageFingerprintBiometricAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }
    }
}
