using Finance.Authentication.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Authentication.Authorization;

/// <summary>
/// Provides and configures all authorization policies for the application.
/// </summary>
public static class AuthorizationPolicyProvider
{
    /// <summary>
    /// Configures all registered policies on the authorization options.
    /// </summary>
    /// <param name="options">The authorization options to configure.</param>
    public static void ConfigurePolicies(IServiceProvider serviceProvider, AuthorizationOptions options)
    {
        // Basic configuration
        new OwnerPolicy(serviceProvider).Configure(serviceProvider, options);
        new AdminPolicy(serviceProvider).Configure(serviceProvider, options);
        new AdminOrOwnerPolicy(serviceProvider).Configure(serviceProvider, options);
        new AuthenticatedPolicy().Configure(serviceProvider, options);
    }
}
