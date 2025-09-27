using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using api.Data;

namespace api.Services;

public interface ICurrentUserService
{
    Guid? GetUserId();
    string? GetUserEntraId();
    string? GetUserPrincipalName();
    string? GetUserDisplayName();
    bool IsAuthenticated();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TodoDbContext _context;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, TodoDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public Guid? GetUserId()
    {
        var entraId = GetUserEntraId();
        if (string.IsNullOrEmpty(entraId))
        {
            return null;
        }

        // Entra IDから実際のUsers.UserIdを取得
        var user = _context.Users
            .Where(u => u.EntraId == entraId && u.IsActive)
            .FirstOrDefault();

        return user?.UserId;
    }

    public string? GetUserEntraId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ??
               _httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value ??
               _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
    }

    public string? GetUserPrincipalName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ??
               _httpContextAccessor.HttpContext?.User?.FindFirst("preferred_username")?.Value;
    }

    public string? GetUserDisplayName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value ??
               _httpContextAccessor.HttpContext?.User?.FindFirst("given_name")?.Value;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
