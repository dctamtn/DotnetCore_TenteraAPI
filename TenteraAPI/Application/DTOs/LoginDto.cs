using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class LoginDto
    {
        [Required, StringLength(100)]
        public string ICNumber { get; set; }
        public string PinHash { get; set; }
        public bool UseFaceBiometric { get; set; }
        public bool UseFingerprintBiometric { get; set; }
    }
}
