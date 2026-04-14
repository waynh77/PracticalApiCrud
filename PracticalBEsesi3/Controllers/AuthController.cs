using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Data;
using PracticalBEsesi3.Dto.Request;
using PracticalBEsesi3.Models;

namespace PracticalBEsesi3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email dan password wajib diisi" });

            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return BadRequest(new { message = "Email sudah terdaftar" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Register berhasil" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized(new { message = "Email salah" });

            bool isValid = BCrypt.Net.BCrypt
                .Verify(dto.Password, user.PasswordHash);

            if (!isValid)
                return Unauthorized(new { message = "Password salah" });

            return Ok(new
            {
                message = "Login berhasil",
                user = new { user.Id, user.Name, user.Email }
            });
        }

    }
}
