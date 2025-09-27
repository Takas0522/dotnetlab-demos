namespace api.DTOs;

public class TodoItemDto
{
    public Guid TodoItemId { get; set; }
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = [];
    public string AccessType { get; set; } = string.Empty; // "Owner" or "Shared"
    public string Permission { get; set; } = string.Empty; // "ReadOnly" or "ReadWrite"
}

public class CreateTodoItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; } = 1;
    public DateTime? DueDate { get; set; }
    public List<Guid> TagIds { get; set; } = [];
}

public class UpdateTodoItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public List<Guid>? TagIds { get; set; }
}

public class TodoItemFilterDto
{
    public bool? IsCompleted { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public List<Guid>? TagIds { get; set; }
    public string? SearchText { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
