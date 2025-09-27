namespace api.DTOs;

public class TagDto
{
    public Guid TagId { get; set; }
    public Guid UserId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? ColorCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UsageCount { get; set; } // このタグが使用されているタスクの数
}

public class CreateTagDto
{
    public string TagName { get; set; } = string.Empty;
    public string? ColorCode { get; set; }
}

public class UpdateTagDto
{
    public string? TagName { get; set; }
    public string? ColorCode { get; set; }
}
