using Finance.Authentication.Authorization.Base;
using Finance.Authentication.Services;
using Finance.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for users who are either Admins or Owners.
/// </summary>
public class AdminOrOwnerPolicy : RolePolicy
{
    public AdminOrOwnerPolicy(IServiceProvider serviceProvider)
        : base("AdminOrOwnerPolicy", serviceProvider, [RoleEnum.Owner])
    {
    }

    protected override bool AssertionAction(AuthorizationHandlerContext context, string userIdClaim)
    {
        using var service = new AuthDbContextService(_serviceProvider);
        var dbContext = service.GetDbContext();

        var userIsAdmin = dbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .Any(u => u.Identities.Any(i => i.SourceId == userIdClaim) && u.Roles.Any(r => r.Id == RoleEnum.Admin));

        if (userIsAdmin)
        {
            return true; // Admin users are always authorized
        }

        // For Owner check, do the same DB query as RolePolicy
        var userIsOwner = dbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .Any(u => u.Identities.Any(i => i.SourceId == userIdClaim) && u.Roles.Any(r => r.Id == RoleEnum.Owner));

        return userIsOwner;
    }
}
