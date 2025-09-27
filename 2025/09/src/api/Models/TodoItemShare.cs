using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("TodoItemShares")]
public class TodoItemShare
{
    [Key]
    public Guid TodoItemShareId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid TodoItemId { get; set; }

    [Required]
    public Guid OwnerUserId { get; set; }

    [Required]
    public Guid SharedUserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Permission { get; set; } = "ReadOnly"; // ReadOnly, ReadWrite

    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(TodoItemId))]
    public virtual TodoItem TodoItem { get; set; } = null!;

    [ForeignKey(nameof(OwnerUserId))]
    public virtual User OwnerUser { get; set; } = null!;

    [ForeignKey(nameof(SharedUserId))]
    public virtual User SharedUser { get; set; } = null!;
}
