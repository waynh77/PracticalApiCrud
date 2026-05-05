using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Data;
using PracticalBEsesi3.Dto.Request;
using PracticalBEsesi3.Models;

namespace PracticalBEsesi3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users/{id} - Get user profile (owner only)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId != id)
            {
                return Forbid();
            }

            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User tidak ditemukan" });
            }

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                TaskCount = user.Tasks?.Count ?? 0
            });
        }

        // PUT: api/Users/{id} - Update user profile
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId != id)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { message = "Nama wajib diisi" });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User tidak ditemukan" });
            }

            user.Name = dto.Name;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil berhasil diperbarui", user.Id, user.Name, user.Email });
        }

        // DELETE: api/Users/{id} - Delete user account
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId != id)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User tidak ditemukan" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Akun berhasil dihapus" });
        }
    }
}
