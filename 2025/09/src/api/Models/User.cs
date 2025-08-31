using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("Users")]
public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string EntraId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string UserPrincipalName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<TodoItem> TodoItems { get; set; } = [];
    public virtual ICollection<Tag> Tags { get; set; } = [];
    public virtual ICollection<TodoItemShare> SharedByMe { get; set; } = [];
    public virtual ICollection<TodoItemShare> SharedWithMe { get; set; } = [];
}
