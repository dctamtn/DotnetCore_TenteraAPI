using TenteraAPI.Domain.Interfaces.Services;
using Twilio.Rest.Api.V2010.Account;
using Twilio;

namespace TenteraAPI.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        public async Task SendVerificationCodeAsync(string phoneNumber, string code)
        {
            try
            {
                TwilioClient.Init("your-account-sid", "your-auth-token");
                await MessageResource.CreateAsync(
                    body: $"Your verification code is {code}. It expires in 10 minutes.",
                    from: new Twilio.Types.PhoneNumber("your-twilio-number"),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );
            }
            catch
            {
                throw new System.Exception("Failed to send SMS");
            }
        }
    }
}
