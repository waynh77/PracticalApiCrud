using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Dto.Request
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Judul wajib diisi")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Judul harus antara 3-200 karakter")]
        public string Judul { get; set; } = "";

        [StringLength(1000, ErrorMessage = "Deskripsi maksimal 1000 karakter")]
        public string? Deskripsi { get; set; }
    }
}
