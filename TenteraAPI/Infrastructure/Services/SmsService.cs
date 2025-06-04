using TenteraAPI.Domain.Interfaces.Services;
using Twilio.Rest.Api.V2010.Account;
using Twilio;
using Twilio.Types;

namespace TenteraAPI.Infrastructure.Services
{
    public interface ITwilioClientWrapper
    {
        Task SendMessageAsync(string to, string from, string body);
    }

    public class TwilioClientWrapper : ITwilioClientWrapper
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioClientWrapper(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;
        }

        public async Task SendMessageAsync(string to, string from, string body)
        {
            TwilioClient.Init(_accountSid, _authToken);
            await MessageResource.CreateAsync(
                body: body,
                from: new PhoneNumber(from),
                to: new PhoneNumber(to)
            );
        }
    }

    public class SmsService : ISmsService
    {
        private readonly string _fromNumber;
        private readonly ITwilioClientWrapper _twilioClient;

        public SmsService(IConfiguration configuration, ITwilioClientWrapper twilioClient = null)
        {
            var accountSid = configuration["TwilioSettings:AccountSid"] 
                ?? throw new ArgumentNullException("TwilioSettings:AccountSid is not configured");
            var authToken = configuration["TwilioSettings:AuthToken"] 
                ?? throw new ArgumentNullException("TwilioSettings:AuthToken is not configured");
            _fromNumber = configuration["TwilioSettings:FromNumber"] 
                ?? throw new ArgumentNullException("TwilioSettings:FromNumber is not configured");

            _twilioClient = twilioClient ?? new TwilioClientWrapper(accountSid, authToken);
        }

        public async Task SendVerificationCodeAsync(string phoneNumber, string code)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number is required");

            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Verification code is required");

            try
            {
                await _twilioClient.SendMessageAsync(
                    phoneNumber,
                    _fromNumber,
                    $"Your verification code is {code}. It expires in 10 minutes."
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send SMS: {ex.Message}", ex);
            }
        }
    }
}
