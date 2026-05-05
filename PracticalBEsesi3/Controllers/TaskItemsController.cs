using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Data;
using PracticalBEsesi3.Dto.Request;
using PracticalBEsesi3.Models;
using System.Security.Claims;

namespace PracticalBEsesi3.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskItemsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            return userId;
        }

        // GET: api/TaskItems
        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = GetUserId();

            var tasks = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .Include(u => u.User)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.IsCompleted,
                    t.CreatedAt,
                    t.UpdatedAt,
                    User = new { t.User!.Id, t.User.Name }
                })
                .ToListAsync();

            return Ok(new { count = tasks.Count, data = tasks });
        }

        // GET: api/TaskItems/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskItem(int id)
        {
            var userId = GetUserId();

            var taskItem = await _context.TaskItems
                .Where(t => t.Id == id && t.UserId == userId)
                .Include(t => t.User)
                .FirstOrDefaultAsync();

            if (taskItem == null)
            {
                return NotFound(new { message = "Task tidak ditemukan" });
            }

            return Ok(new
            {
                taskItem.Id,
                taskItem.Title,
                taskItem.Description,
                taskItem.IsCompleted,
                taskItem.CreatedAt,
                taskItem.UpdatedAt,
                User = new { taskItem.User!.Id, taskItem.User.Name }
            });
        }

        // POST: api/TaskItems
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();

            var task = new TaskItem
            {
                Title = dto.Judul,
                Description = dto.Deskripsi,
                IsCompleted = false,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItem), new { id = task.Id }, new
            {
                task.Id,
                task.Title,
                task.Description,
                task.IsCompleted,
                task.CreatedAt,
                task.UpdatedAt
            });
        }

        // PUT: api/TaskItems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == 0)
            {
                return BadRequest(new { message = "ID tidak valid" });
            }

            var userId = GetUserId();

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound(new { message = "Task tidak ditemukan" });
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                task.Id,
                task.Title,
                task.Description,
                task.IsCompleted,
                task.CreatedAt,
                task.UpdatedAt
            });
        }

        // DELETE: api/TaskItems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var userId = GetUserId();

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound(new { message = "Task tidak ditemukan" });
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
