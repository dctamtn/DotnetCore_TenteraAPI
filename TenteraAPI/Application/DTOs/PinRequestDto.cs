using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class PinRequestDto
    {
        [StringLength(15)]
        public string ICNumber { get; set; }
        public string PinHash { get; set; }
    }
}
