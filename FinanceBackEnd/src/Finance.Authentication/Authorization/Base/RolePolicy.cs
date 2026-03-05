using Finance.Authentication.Services;
using Finance.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Finance.Authentication.Authorization.Base;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public abstract class RolePolicy : BasicPolicy
{
    protected ICollection<RoleEnum> _roles;
    protected IServiceProvider _serviceProvider;

    protected RolePolicy(string name, IServiceProvider serviceProvider, ICollection<RoleEnum> roles)
        : base(name)
    {
        _roles = roles;
        _serviceProvider = serviceProvider;
    }

    protected override bool AssertionAction(AuthorizationHandlerContext context, string userIdClaim)
    {
        using var service = new AuthDbContextService(_serviceProvider);
        var dbContext = service.GetDbContext();
        var dbUser = dbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .FirstOrDefault(u => u.Identities.Any(i => i.SourceId == userIdClaim));

        return dbUser != null && dbUser.Roles.Any(r => _roles.Contains(r.Id));
    }
}
