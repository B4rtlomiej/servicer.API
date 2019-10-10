using System.ComponentModel.DataAnnotations;

namespace servicer.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters.")]
        public string Username { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password must be more than 4 characters.")]
        public string Password { get; set; }
    }
}