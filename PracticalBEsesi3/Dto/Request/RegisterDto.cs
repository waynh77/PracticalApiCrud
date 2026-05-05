using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Dto.Request
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Nama wajib diisi")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nama harus antara 3-100 karakter")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [StringLength(255, ErrorMessage = "Email maksimal 255 karakter")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = "";
    }
}
