namespace api.DTOs;

public class TodoItemShareDto
{
    public Guid TodoItemShareId { get; set; }
    public Guid TodoItemId { get; set; }
    public string TodoItemTitle { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string OwnerDisplayName { get; set; } = string.Empty;
    public Guid SharedUserId { get; set; }
    public string SharedUserDisplayName { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTodoItemShareDto
{
    public Guid TodoItemId { get; set; }
    public string SharedWithEmail { get; set; } = string.Empty;
    public string Permission { get; set; } = "ReadOnly"; // ReadOnly, ReadWrite
}

public class UpdateTodoItemShareDto
{
    public string? Permission { get; set; }
    public bool? IsActive { get; set; }
}
