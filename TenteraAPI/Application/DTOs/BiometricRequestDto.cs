using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class BiometricRequestDto
    {
        [Required, StringLength(100)]
        public string ICNumber { get; set; }
        [Required]
        public bool Enable { get; set; }
    }
}
