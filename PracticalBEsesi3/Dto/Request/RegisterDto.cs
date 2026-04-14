using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Dto.Request
{
    public class RegisterDto
    {
        public string Name { get; set; } = "";
        [Required]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
