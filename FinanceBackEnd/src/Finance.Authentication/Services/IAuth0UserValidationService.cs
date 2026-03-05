namespace Finance.Authentication.Services;

/// <summary>
/// Service for validating users in Auth0.
/// </summary>
public interface IAuth0UserValidationService
{
    /// <summary>
    /// Validates that a user exists in Auth0.
    /// </summary>
    /// <param name="userId">The Auth0 user ID to validate.</param>
    /// <returns>True if the user exists in Auth0, false otherwise.</returns>
    Task<bool> ValidateUserExistsAsync(string userId);

    /// <summary>
    /// Gets user information from Auth0.
    /// </summary>
    /// <param name="userId">The Auth0 user ID.</param>
    /// <returns>User information if found, null otherwise.</returns>
    Task<Auth0UserInfo?> GetUserInfoAsync(string userId);
}
