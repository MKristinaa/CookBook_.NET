using System.ComponentModel.DataAnnotations;

namespace Backend.Dto
{
    public class LoginDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Token { get; set; }
    }
}
