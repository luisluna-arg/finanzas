namespace Finance.Authentication.Options;

/// <summary>
/// Configuration options for Auth0 authentication and management.
/// </summary>
public class Auth0Options
{
    public const string SectionName = "Auth0";

    /// <summary>
    /// Gets or sets the Auth0 domain (e.g., "your-domain.auth0.com").
    /// Required for user validation.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 audience for API authentication.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 application configuration.
    /// </summary>
    public ApplicationOptions Application { get; set; } = new();

    /// <summary>
    /// Gets or sets the Auth0 Management API configuration.
    /// </summary>
    public ManagementApiOptions ManagementApi { get; set; } = new();
}

/// <summary>
/// Configuration options for Auth0 Application.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Gets or sets the Auth0 application client ID.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 application client secret.
    /// </summary>
    public string? ClientSecret { get; set; }
}

/// <summary>
/// Configuration options for Auth0 Management API.
/// </summary>
public class ManagementApiOptions
{
    /// <summary>
    /// Gets or sets the Auth0 Management API Client ID.
    /// Required for validating users exist in Auth0.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 Management API Client Secret.
    /// Required for validating users exist in Auth0.
    /// </summary>
    public string? ClientSecret { get; set; }
}
