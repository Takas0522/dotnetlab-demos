using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<TodoAttachment> TodoAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Todos
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Todos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Todos)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // TodoAttachments
            modelBuilder.Entity<TodoAttachment>(entity =>
            {
                entity.HasOne(d => d.Todo)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.TodoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
