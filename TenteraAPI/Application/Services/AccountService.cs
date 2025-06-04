using TenteraAPI.Application.DTOs;
using TenteraAPI.Domain.Entities;
using TenteraAPI.Domain.Interfaces.Repositories;
using TenteraAPI.Domain.Interfaces.Services;
using System.Text.RegularExpressions;

namespace TenteraAPI.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IVerificationCodeStore _codeStore;

        public AccountService(
            IAccountRepository accountRepository,
            IEmailService emailService,
            ISmsService smsService,
            IVerificationCodeStore codeStore)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _smsService = smsService;
            _codeStore = codeStore;
        }

        public async Task<(bool Success, string Message, int? CustomerId)> RegisterAsync(AccountRegistrationDto dto)
        {
			if (string.IsNullOrEmpty(dto.CustomerName))
				return (false, "Customer Name is required", null);

			if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required", null);

            if (string.IsNullOrEmpty(dto.Email))
                return (false, "Email is required", null);

            if (string.IsNullOrEmpty(dto.PhoneNumber))
                return (false, "Phone number is required", null);

            if (!IsValidEmail(dto.Email))
                return (false, "Invalid email format", null);

            if (!IsValidPhoneNumber(dto.PhoneNumber))
                return (false, "Invalid phone number format", null);

            if (await _accountRepository.ICNumberExistsAsync(dto.ICNumber))
                return (false, "ICNumber already registered", null);

            if (await _accountRepository.EmailExistsAsync(dto.Email))
                return (false, "Email already registered", null);

            if (await _accountRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
                return (false, "Phone number already registered", null);

            var account = new Account
            {
                CustomerName = dto.CustomerName,
                ICNumber = dto.ICNumber,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                HasAcceptedPrivacyPolicy = dto.HasAcceptedPrivacyPolicy,
                PinHash = string.Empty,
                IsEmailVerified = false,
                IsPhoneVerified = false,
                UseFaceBiometric = false,
                UseFingerprintBiometric = false,
                IsFaceBiometricEnabled = false,
                IsFingerprintBiometricEnabled = false
            };

            await _accountRepository.AddAsync(account);

            return (true, "Customer registered successfully.", account.Id);
        }

        public async Task<(bool Success, string Message)> SendEmailVerificationCodeAsync(RequestSendCodeDto info)
        {
            if (string.IsNullOrEmpty(info.ICNumber))
                return (false, "ICNumber is required");

            var account = await _accountRepository.GetICNumberAsync(info.ICNumber);
            if (account is null)
                return (false, "Account not found");

            string code = GenerateCode();
            await _codeStore.StoreCodeAsync(account.Email, code, DateTime.UtcNow.AddMinutes(10));
            await _emailService.SendVerificationCodeAsync(account.Email, code);
            return (true, "Email verification code sent");
        }

        public async Task<(bool Success, string Message)> SendMobileVerificationCodeAsync(RequestSendCodeDto info)
        {
            if (string.IsNullOrEmpty(info.ICNumber))
                return (false, "ICNumber is required");

            var account = await _accountRepository.GetICNumberAsync(info.ICNumber);
            if (account is null)
                return (false, "Account not found");

            string code = GenerateCode();
            await _codeStore.StoreCodeAsync(account.PhoneNumber, code, DateTime.UtcNow.AddMinutes(10));
            await _smsService.SendVerificationCodeAsync(account.PhoneNumber, code);
            return (true, "Mobile verification code sent");
        }

        public async Task<(bool Success, string Message)> VerifyCodeAsync(VerifyCodeDto dto)
        {
            if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required");

            if (string.IsNullOrEmpty(dto.Code))
                return (false, "Verification code is required");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account is null)
                return (false, "Account not found");

            string key = dto.Type == TypeVerify.EMAIL ? account.Email : account.PhoneNumber;
            var stored = await _codeStore.GetCodeAsync(key);
            if (!stored.HasValue || stored.Value.Expiry < DateTime.UtcNow)
                return (false, "Invalid or expired code");

            if (stored.Value.Code != dto.Code)
                return (false, "Incorrect code");

            await _codeStore.RemoveCodeAsync(key);
            if (dto.Type == TypeVerify.EMAIL)
            {
                account.IsEmailVerified = true;
            } 
            else
            {
                account.IsPhoneVerified = true;
            }
            await _accountRepository.UpdateAsync(account);
            return (true, "Code verified successfully");
        }

        public async Task<(bool Success, string Message)> CreatePin(PinRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required");

            if (string.IsNullOrEmpty(dto.PinHash))
                return (false, "PIN is required");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Account not found");

            if (!account.IsEmailVerified)
                return (false, "Email has not been verified yet");

            if (!account.IsPhoneVerified)
                return (false, "Phone number has not been verified yet");

            if (!account.HasAcceptedPrivacyPolicy)
                return (false, "Privacy Policy has not been accepted");

            account.PinHash = dto.PinHash;
            await _accountRepository.UpdateAsync(account);
            return (true, "PIN created successfully");
        }

        public async Task<(bool Success, string Message, int? CustomerId)> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required", null);

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Account not found", null);

            if (!account.IsEmailVerified)
                return (false, "Email has not been verified yet", null);

            if (!account.IsPhoneVerified)
                return (false, "Phone number has not been verified yet", null);

            if (!account.HasAcceptedPrivacyPolicy)
                return (false, "Privacy Policy has not been accepted", null);

            if (string.IsNullOrEmpty(account.PinHash))
                return (false, "PIN has not been set up", null);

            if (dto.PinHash != account.PinHash)
                return (false, "Invalid PIN", null);

            if (dto.UseFaceBiometric && !account.IsFaceBiometricEnabled)
                return (false, "Face biometric login not enabled for this account", null);

            if (dto.UseFingerprintBiometric && !account.IsFingerprintBiometricEnabled)
                return (false, "Fingerprint biometric login not enabled for this account", null);

            return (true, "Login successful", account.Id);
        }

        public async Task<(bool Success, string Message)> ManageFaceBiometricAsync(BiometricRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Account not found");

            account.IsFaceBiometricEnabled = dto.Enable;
            account.UseFaceBiometric = dto.Enable;
            await _accountRepository.UpdateAsync(account);
            return (true, $"Face biometric {(dto.Enable ? "enabled" : "disabled")} successfully");
        }

        public async Task<(bool Success, string Message)> ManageFingerprintBiometricAsync(BiometricRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.ICNumber))
                return (false, "ICNumber is required");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Account not found");

            account.IsFingerprintBiometricEnabled = dto.Enable;
            account.UseFingerprintBiometric = dto.Enable;
            await _accountRepository.UpdateAsync(account);
            return (true, $"Fingerprint biometric {(dto.Enable ? "enabled" : "disabled")} successfully");
        }

        private string GenerateCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+\d{10,15}$");
        }
    }
}