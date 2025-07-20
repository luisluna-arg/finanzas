using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Authentication.Authorization.Base;

/// <summary>
/// Basic authentication policy.
/// </summary>
public abstract class BasicPolicy : IAuthorizationPolicy
{
    protected BasicPolicy(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the policy name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Configures the Basic policy.
    /// </summary>
    /// <param name="options">The authorization options to configure.</param>
    public virtual void Configure(AuthorizationOptions options)
    {
        // For now, just require authentication
        // This is a temporary simplification until roles are properly configured
        options.AddPolicy(Name, policy =>
            policy.RequireAssertion(context => CheckAuthenticated(context)));
    }

    protected virtual bool CheckAuthenticated(AuthorizationHandlerContext context)
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

        return AssertionAction(context, userIdClaim.Value);
    }

    protected virtual bool AssertionAction(AuthorizationHandlerContext context, string userIdClaim)
    {
        return true; // Default implementation allows all authenticated users
    }
}
