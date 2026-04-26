using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Models;

namespace PracticalBEsesi3.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder mb)
        {

            mb.Entity<TaskItem>()
                .HasOne(k => k.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(t => t.UserId);

            mb.Entity<User>()
                .HasMany(t => t.Tasks)
                .WithOne(u => u.User)
                .HasForeignKey(f => f.UserId);
                
        }
    }
}
