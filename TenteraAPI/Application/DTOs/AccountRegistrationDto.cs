using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class AccountRegistrationDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [StringLength(15)]
        public string ICNumber { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }
    }
}
