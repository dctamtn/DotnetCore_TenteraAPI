using TenteraAPI.Application.DTOs;
using TenteraAPI.Domain.Entities;
using TenteraAPI.Domain.Interfaces.Repositories;
using TenteraAPI.Domain.Interfaces.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TenteraAPI.Application.Services
{
    public class AccountService
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
            if (!string.IsNullOrEmpty(dto.ICNumber))
                return (false, "Invalid ICNumber", null);

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                return (false, "Invalid email format", null);

            if (!string.IsNullOrEmpty(dto.PhoneNumber) && !IsValidPhoneNumber(dto.PhoneNumber))
                return (false, "Invalid phone number format", null);

            if (await _accountRepository.ICNumberExistsAsync(dto.ICNumber))
                return (false, "ICNumber already registered", null);

            if (await _accountRepository.EmailExistsAsync(dto.Email))
                return (false, "Email already registered", null);

            if (!string.IsNullOrEmpty(dto.PhoneNumber) && await _accountRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
                return (false, "Phone number already registered", null);

            var account = new Account
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ICNumber = dto.ICNumber,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            await _accountRepository.AddAsync(account);

            return (true, "Customer registered successfully, verification codes sent", account.Id);
        }

        public async Task<(bool Success, string Message)> SendEmailVerificationCodeAsync(RequestSendCodeDto info)
        {
            var account = await _accountRepository.GetICNumberAsync(info.ICNumber);
            if (account is null)
                return (false, "account not found");

            string code = GenerateCode();
            await _codeStore.StoreCodeAsync(account.Email, code, DateTime.UtcNow.AddMinutes(10));
            await _emailService.SendVerificationCodeAsync(account.Email, code);
            return (true, "Email verification code sent");
        }

        public async Task<(bool Success, string Message)> SendMobileVerificationCodeAsync(RequestSendCodeDto info)
        {
            var account = await _accountRepository.GetICNumberAsync(info.ICNumber);
            if (account is null)
                return (false, "account not found");

            string code = GenerateCode();
            await _codeStore.StoreCodeAsync(account.PhoneNumber, code, DateTime.UtcNow.AddMinutes(10));
            await _smsService.SendVerificationCodeAsync(account.PhoneNumber, code);
            return (true, "Mobile verification code sent");
        }

        public async Task<(bool Success, string Message)> VerifyCodeAsync(VerifyCodeDto dto)
        {
            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account is null)
                return (false, "account not found");

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
            return (true, "Code verified successfully");
        }

        public async Task<(bool Success, string Message)> CreatePin(PinRequestDto dto)
        {
            if (!string.IsNullOrEmpty(dto.PinHash))
                return (false, "Invalid Pin code");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "The IC number has not been registered yet");

            if (!account.IsEmailVerified)
                return (false, "Email has not verifed yet");

            if (!account.IsPhoneVerified)
                return (false, "PhoneNumber has not verifed yet");

            if (!account.HasAcceptedPrivacyPolicy)
                return (false, "Privacy Policy has not accepted");
            account.PinHash = dto.PinHash;

            return (true, "created Pin code");
        }

        public async Task<(bool Success, string Message, int? CustomerId)> LoginAsync(LoginDto dto)
        {
            if (!String.IsNullOrEmpty(dto.ICNumber))
                return (false, "Invalid ICNumber", null);

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "The IC number has not been registered yet", null);

            if (!account.IsEmailVerified)
                return (false, "Email has not verifed yet", null);

            if (!account.IsPhoneVerified)
                return (false, "PhoneNumber has not verifed yet", null);

            if (!account.HasAcceptedPrivacyPolicy)
                return (false, "Privacy Policy has not accepted", null);

            if (!String.IsNullOrEmpty(account.PinHash) && dto.PinHash == account.PinHash)
                return (false, "Pin code has not valid", null);

            if (!account.UseFaceBiometric && !account.UseFingerprintBiometric)
                return (false, "Biometric has not been registered yet", null);

            if (account.UseFaceBiometric && !account.IsFaceBiometricEnabled)
                return (false, "Face biometric login not enabled for this account", null);

            if (account.UseFingerprintBiometric && !account.IsFingerprintBiometricEnabled)
                return (false, "Fingerprint biometric login not enabled for this account", null);

            return (true, "Login successful", account.Id);
        }

        public async Task<(bool Success, string Message)> ManageFaceBiometricAsync(BiometricRequestDto dto)
        {
            if (!String.IsNullOrEmpty(dto.ICNumber))
                return (false, "Invalid ICNumber");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Customer not found");
            account.IsFaceBiometricEnabled = dto.Enable;
            account.UseFaceBiometric = account.IsFaceBiometricEnabled;
            await _accountRepository.UpdateAsync(account);
            return (true, $"Face biometric {(dto.Enable ? "enabled" : "disabled")} successfully");
        }

        public async Task<(bool Success, string Message)> ManageFingerprintBiometricAsync(BiometricRequestDto dto)
        {
            if (!String.IsNullOrEmpty(dto.ICNumber))
                return (false, "Invalid ICNumber");

            var account = await _accountRepository.GetICNumberAsync(dto.ICNumber);
            if (account == null)
                return (false, "Customer not found");

            account.IsFingerprintBiometricEnabled = dto.Enable;
            account.UseFingerprintBiometric = account.IsFingerprintBiometricEnabled;
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
