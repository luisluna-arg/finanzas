using System.Security.Claims;
using Finance.Domain.Enums;
using Finance.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Finance.Authentication.Authorization.Base;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public abstract class RolePolicy : IAuthorizationPolicy
{
    protected RolePolicy(string name, ICollection<RoleEnum> roles, FinanceDbContext dbContext)
    {
        Name = name;
        Roles = roles;
        DbContext = dbContext;
    }

    /// <summary>
    /// Gets the policy name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the policy Roles.
    /// </summary>
    public ICollection<RoleEnum> Roles { get; }

    /// <summary>
    /// Gets the database context for database-based authorization decisions.
    /// </summary>
    protected FinanceDbContext DbContext { get; private set; }

    /// <summary>
    /// Configures the Owner policy.
    /// </summary>
    /// <param name="options">The authorization options to configure.</param>
    public virtual void Configure(AuthorizationOptions options)
    {
        // For now, just require authentication
        // This is a temporary simplification until roles are properly configured
        options.AddPolicy(Name, policy =>
            policy.RequireAssertion(context => AssertionAction(context)));
    }

    protected virtual bool AssertionAction(AuthorizationHandlerContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return false; // User is not authenticated
        }

        // Use DbContext to verify if user is an owner
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return false; // No user ID claim
        }

        var auth0UserId = userIdClaim.Value;

        // This needs to be executed synchronously in an assertion
        var dbUser = DbContext.User
            .Include(u => u.Identities)
            .Include(u => u.Roles)
            .FirstOrDefault(u => u.Identities.Any(i => i.SourceId == auth0UserId));

        return dbUser != null && dbUser.Roles.Any(r => Roles.Contains(r.Id));
    }
}
