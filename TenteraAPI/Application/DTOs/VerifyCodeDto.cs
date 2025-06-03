using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public enum TypeVerify
    {
        PHONE = 0,
        EMAIL = 1
    }
    public class VerifyCodeDto
    {
        [Required]
        public TypeVerify Type { get; set; }

        [StringLength(15)]
        public string ICNumber { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
