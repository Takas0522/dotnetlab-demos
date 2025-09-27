using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("TodoItemTags")]
public class TodoItemTag
{
    [Key]
    public Guid TodoItemTagId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid TodoItemId { get; set; }

    [Required]
    public Guid TagId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(TodoItemId))]
    public virtual TodoItem TodoItem { get; set; } = null!;

    [ForeignKey(nameof(TagId))]
    public virtual Tag Tag { get; set; } = null!;
}
