using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
