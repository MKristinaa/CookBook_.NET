using System.ComponentModel.DataAnnotations;

namespace Backend.Dto
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Image { get; set; }
        public bool? Verified { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
