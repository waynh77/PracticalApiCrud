using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Dto.Request
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Nama wajib diisi")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nama harus antara 3-100 karakter")]
        public string Name { get; set; } = "";
    }
}
