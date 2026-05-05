using Microsoft.EntityFrameworkCore;
using PracticalBEsesi3.Models;

namespace PracticalBEsesi3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User to Tasks relationship with cascade delete
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for better query performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.UserId);

            // Configure property constraints
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Title)
                .HasMaxLength(200);
        }
    }
}
