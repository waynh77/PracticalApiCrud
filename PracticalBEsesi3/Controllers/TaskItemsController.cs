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

        //// GET: api/TaskItems
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        //{
        //    return await _context.TaskItems
        //        .Include(t=>t.User).ToListAsync();
        //}

        // GET: api/TaskItems/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        //{
        //    var taskItem = await _context.TaskItems
        //        .FindAsync(id);

        //    if (taskItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return taskItem;
        //}

        // PUT: api/TaskItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTaskItem(int id, UpdateTaskDto taskItem)
        //{
        //    if (id==0)
        //    {
        //        return BadRequest();
        //    }
        //    var task = await _context.TaskItems.FindAsync(id);
        //    if (task == null)
        //        return NotFound(new { message = "Task tidak ditemukan" });

        //    if (string.IsNullOrWhiteSpace(taskItem.Title))
        //        return BadRequest(new { message = "Title wajib diisi" });

        //    task.Title = taskItem.Title;
        //    task.Description = taskItem.Description;
        //    task.IsCompleted = taskItem.IsCompleted;
        //    task.UpdateAt=DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTaskItem", new { id = task.Id }, task);
        //}

        // POST: api/TaskItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<TaskItem>> PostTaskItem(CreateTaskDto taskItem)
        //{
        //    var task = new TaskItem
        //    {
        //        Id=0,
        //        Description = taskItem.Deskripsi,
        //        Title = taskItem.Judul
        //    };
        //    _context.TaskItems.Add(task);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTaskItem", new { id = task.Id }, task);
        //}

        // DELETE: api/TaskItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound(new { message = "Task tidak ditemukan" });


            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tasks = await _context.TaskItems
                .Include(u=>u.User)
                .Where(t => t.UserId == userId)
                .Select(t => new TaskItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    User=t.User
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = new TaskItem
            {
                Title = dto.Judul,
                Description = dto.Deskripsi,
                IsCompleted = false,
                UserId = userId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound(new { message = "Task tidak ditemukan" });

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            await _context.SaveChangesAsync();
            return Ok(task);
        }

    }
}
