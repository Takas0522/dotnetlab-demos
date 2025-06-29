namespace TodoApi.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte Priority { get; set; } = 1;
        public byte Status { get; set; } = 1;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
    }

    public class CreateTodoDto
    {
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte Priority { get; set; } = 1;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTodoDto
    {
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public byte? Priority { get; set; }
        public byte? Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
