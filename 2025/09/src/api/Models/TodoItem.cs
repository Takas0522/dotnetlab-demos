using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("TodoItems")]
public class TodoItem
{
    [Key]
    public Guid TodoItemId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    [Range(1, 3)]
    public int Priority { get; set; } = 1; // 1: 低, 2: 中, 3: 高

    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<TodoItemTag> TodoItemTags { get; set; } = [];
    public virtual ICollection<TodoItemShare> TodoItemShares { get; set; } = [];
}
