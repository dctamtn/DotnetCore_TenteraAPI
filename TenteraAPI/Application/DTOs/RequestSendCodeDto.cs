using System.ComponentModel.DataAnnotations;

namespace TenteraAPI.Application.DTOs
{
    public class RequestSendCodeDto
    {
        [StringLength(15)]
        public string ICNumber { get; set; }
    }
}
