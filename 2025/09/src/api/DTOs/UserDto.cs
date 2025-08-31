namespace api.DTOs;

public class UserDto
{
    public Guid UserId { get; set; }
    public string EntraId { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    public string EntraId { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class UpdateUserDto
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}
