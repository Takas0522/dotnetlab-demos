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
        // まずカスタムUserIdクレームを確認
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        // Azure AD v2.0のoidクレームを確認（標準形式）
        var oidClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value;
        if (Guid.TryParse(oidClaim, out var oid))
        {
            return oid;
        }

        // Azure AD v2.0のobjectidentifierクレームを確認（完全URI形式）
        var objectIdentifierClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        if (Guid.TryParse(objectIdentifierClaim, out var objectIdentifier))
        {
            return objectIdentifier;
        }

        return null;
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
