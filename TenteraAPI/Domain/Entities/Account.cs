namespace TenteraAPI.Domain.Entities
{
    public class Account {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ICNumber { get; set; }
        public string PinHash { get; set; }
        public bool UseFaceBiometric { get; set; }
        public bool UseFingerprintBiometric { get; set; }
        public bool IsFaceBiometricEnabled { get; set; }
        public bool IsFingerprintBiometricEnabled { get; set; }
        public bool HasAcceptedPrivacyPolicy { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
    } 
}
