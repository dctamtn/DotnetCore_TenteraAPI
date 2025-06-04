using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class AccountRegistrationDto
    {
        [Required, StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(15)]
        public string ICNumber { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public bool HasAcceptedPrivacyPolicy { get; set; }
    }
}
