using System.ComponentModel.DataAnnotations;

namespace Backend.Dto
{
    public class loginInfoDto
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
