using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;
using Finance.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for users who are either Admins or Owners.
/// </summary>
public class AdminOrOwnerPolicy : RolePolicy
{
    public AdminOrOwnerPolicy(FinanceDbContext dbContext)
        : base("AdminOrOwnerPolicy", [RoleEnum.Owner], dbContext)
    {
    }

    protected override bool AssertionAction(AuthorizationHandlerContext context, string userIdClaim)
    {
        // This needs to be executed synchronously in an assertion
        var userIsAdmin = DbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .Any(u => u.Identities.Any(i => i.SourceId == userIdClaim) && u.Roles.Any(r => r.Id == RoleEnum.Admin));

        if (userIsAdmin)
        {
            return true; // Admin users are always authorized
        }

        return base.AssertionAction(context, userIdClaim);
    }
}
