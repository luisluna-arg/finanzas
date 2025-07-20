using Finance.Domain.Enums;
using Finance.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Finance.Authentication.Authorization.Base;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public abstract class RolePolicy : BasicPolicy
{
    protected RolePolicy(string name, ICollection<RoleEnum> roles, FinanceDbContext dbContext)
        : base(name)
    {
        Roles = roles;
        DbContext = dbContext;
    }

    /// <summary>
    /// Gets the policy Roles.
    /// </summary>
    public ICollection<RoleEnum> Roles { get; }

    /// <summary>
    /// Gets the database context for database-based authorization decisions.
    /// </summary>
    protected FinanceDbContext DbContext { get; private set; }

    protected override bool AssertionAction(AuthorizationHandlerContext context, string userIdClaim)
    {
        // This needs to be executed synchronously in an assertion
        var dbUser = DbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .FirstOrDefault(u => u.Identities.Any(i => i.SourceId == userIdClaim));

        return dbUser != null && dbUser.Roles.Any(r => Roles.Contains(r.Id));
    }
}
