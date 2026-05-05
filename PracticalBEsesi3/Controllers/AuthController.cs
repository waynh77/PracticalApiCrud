using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticalBEsesi3.Data;
using PracticalBEsesi3.Dto.Request;
using PracticalBEsesi3.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PracticalBEsesi3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Nama wajib diisi" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email wajib diisi" });

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return BadRequest(new { message = "Password minimal 6 karakter" });

            // Email format validation
            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(dto.Email))
                return BadRequest(new { message = "Format email tidak valid" });

            // Check if email already exists
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (existingUser != null)
                return BadRequest(new { message = "Email sudah terdaftar" });

            try
            {
                var user = new User
                {
                    Name = dto.Name.Trim(),
                    Email = dto.Email.ToLower().Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                return Created(string.Empty, new { message = "Register berhasil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat register", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email dan password wajib diisi" });

            try
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (user == null)
                    return Unauthorized(new { message = "Email atau password salah" });

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!isValidPassword)
                    return Unauthorized(new { message = "Email atau password salah" });

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "Login berhasil",
                    user = new { user.Id, user.Name, user.Email },
                    token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat login", error = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];
            var jwtExpireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var expireMinutes) ? expireMinutes : 60;

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
