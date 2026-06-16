using Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Auth;

public class IsAdminUser(IHttpContextAccessor httpContextAccessor, FinanceDbContext db)
{
    private (bool IsAdmin, Guid? UserId)? _cache;

    public async Task<(bool IsAdmin, Guid? UserId)> IsSatisfiedAsync(CancellationToken ct = default)
    {
        if (_cache.HasValue) return _cache.Value;

        var identitySourceId = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(identitySourceId))
        {
            _cache = (false, null);
            return _cache.Value;
        }

        var user = await db.User
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == identitySourceId), ct);

        _cache = (user?.Roles.Any(r => r.Id == RoleEnum.Admin) ?? false, user?.Id);
        return _cache.Value;
    }
}
