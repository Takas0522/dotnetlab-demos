using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("todo_attachments")]
    public class TodoAttachment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("todo_id")]
        public int TodoId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Column("file_size")]
        public long FileSize { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("content_type")]
        public string ContentType { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TodoId")]
        public virtual Todo Todo { get; set; } = null!;
    }
}
