using Finance.Authentication.Authorization.Policies;
using Finance.Persistance;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Authentication.Authorization;

/// <summary>
/// Provides and configures all authorization policies for the application.
/// </summary>
public static class AuthorizationPolicyProvider
{
    /// <summary>
    /// Configures all registered policies on the authorization options with access to the database context.
    /// </summary>
    /// <param name="options">The authorization options to configure.</param>
    /// <param name="dbContext">The database context to use for data-based authorization decisions.</param>
    public static void ConfigurePoliciesWithDb(AuthorizationOptions options, FinanceDbContext dbContext)
    {
        // Basic configuration
        new OwnerPolicy(dbContext).Configure(options);
        new AdminPolicy(dbContext).Configure(options);
        new AdminOrOwnerPolicy(dbContext).Configure(options);
        new AuthenticatedPolicy().Configure(options);
    }
}
