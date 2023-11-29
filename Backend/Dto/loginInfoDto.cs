using System.ComponentModel.DataAnnotations;

namespace Backend.Dto
{
    public class loginInfoDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
