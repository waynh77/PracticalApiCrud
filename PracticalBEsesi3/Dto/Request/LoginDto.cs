using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Dto.Request
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = "";
    }
}
