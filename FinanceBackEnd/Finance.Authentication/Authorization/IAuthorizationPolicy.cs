using Microsoft.AspNetCore.Authorization;

namespace Finance.Authentication.Authorization;

/// <summary>
/// Interface for authorization policies.
/// </summary>
public interface IAuthorizationPolicy
{
    /// <summary>
    /// Gets the policy name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Configures the policy.
    /// </summary>
    /// <param name="options">The authorization options to configure.</param>
    void Configure(AuthorizationOptions options);
}
