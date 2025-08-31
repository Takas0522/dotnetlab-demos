using System.Security.Claims;

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

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public string? GetUserEntraId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
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
