using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Data;
using PracticalBEsesi3.Dto.Request;
using PracticalBEsesi3.Models;

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

        // GET: api/TaskItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        {
            return await _context.TaskItems.ToListAsync();
        }

        // GET: api/TaskItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // PUT: api/TaskItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, UpdateTaskDto taskItem)
        {
            if (id==0)
            {
                return BadRequest();
            }
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "Task tidak ditemukan" });

            if (string.IsNullOrWhiteSpace(taskItem.Title))
                return BadRequest(new { message = "Title wajib diisi" });

            task.Title = taskItem.Title;
            task.Description = taskItem.Description;
            task.IsCompleted = taskItem.IsCompleted;
            task.UpdateAt=DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskItem", new { id = task.Id }, task);
        }

        // POST: api/TaskItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(CreateTaskDto taskItem)
        {
            var task = new TaskItem
            {
                Id=0,
                Description = taskItem.Deskripsi,
                Title = taskItem.Judul
            };
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskItem", new { id = task.Id }, task);
        }

        // DELETE: api/TaskItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
