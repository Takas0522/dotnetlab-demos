using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TodoItemTag> TodoItemTags { get; set; }
    public DbSet<TodoItemShare> TodoItemShares { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User エンティティの設定
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.EntraId).IsUnique();
            entity.HasIndex(e => e.UserPrincipalName);
            entity.HasIndex(e => e.IsActive);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // TodoItem エンティティの設定
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.TodoItemId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsCompleted);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.UserId, e.IsCompleted, e.IsDeleted });
            entity.HasIndex(e => new { e.UserId, e.IsDeleted, e.CreatedAt });
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.TodoItems)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Tag エンティティの設定
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => new { e.UserId, e.IsDeleted });
            entity.HasIndex(e => new { e.UserId, e.TagName }).IsUnique()
                  .HasFilter("[IsDeleted] = 0"); // 論理削除されていないもののみユニーク制約
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TodoItemTag エンティティの設定
        modelBuilder.Entity<TodoItemTag>(entity =>
        {
            entity.HasKey(e => e.TodoItemTagId);
            entity.HasIndex(e => e.TodoItemId);
            entity.HasIndex(e => e.TagId);
            entity.HasIndex(e => new { e.TodoItemId, e.TagId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.TodoItem)
                  .WithMany(t => t.TodoItemTags)
                  .HasForeignKey(e => e.TodoItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                  .WithMany(t => t.TodoItemTags)
                  .HasForeignKey(e => e.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TodoItemShare エンティティの設定
        modelBuilder.Entity<TodoItemShare>(entity =>
        {
            entity.HasKey(e => e.TodoItemShareId);
            entity.HasIndex(e => e.TodoItemId);
            entity.HasIndex(e => e.OwnerUserId);
            entity.HasIndex(e => e.SharedUserId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.SharedUserId, e.IsActive });
            entity.HasIndex(e => new { e.TodoItemId, e.SharedUserId }).IsUnique();
            entity.Property(e => e.SharedAt).HasDefaultValueSql("GETUTCDATE()");

            // 権限の制約
            entity.Property(e => e.Permission)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // 自分自身への共有を防ぐ制約はアプリケーションレベルで実装

            entity.HasOne(e => e.TodoItem)
                  .WithMany(t => t.TodoItemShares)
                  .HasForeignKey(e => e.TodoItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.OwnerUser)
                  .WithMany(u => u.SharedByMe)
                  .HasForeignKey(e => e.OwnerUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SharedUser)
                  .WithMany(u => u.SharedWithMe)
                  .HasForeignKey(e => e.SharedUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is TodoItem todoItem)
            {
                todoItem.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Tag tag)
            {
                tag.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is User user)
            {
                user.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
