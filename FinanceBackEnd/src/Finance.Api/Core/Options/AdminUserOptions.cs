namespace Finance.Api.Core.Options;

/// <summary>
/// Configuration options for admin user management.
/// </summary>
public class AdminUserOptions
{
    public const string SectionName = "AdminUser";

    /// <summary>
    /// Gets or sets the Auth0 User ID for the admin user. Should be retrieved from secure configuration.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable automatic admin user seeding.
    /// Should be false in production environments.
    /// </summary>
    public bool EnableSeeding { get; set; } = true;

    /// <summary>
    /// Gets or sets the default username for the admin user if not available from identity provider.
    /// </summary>
    public string DefaultUsername { get; set; } = "Admin";

    /// <summary>
    /// Gets or sets the default first name for the admin user if not available from identity provider.
    /// </summary>
    public string DefaultFirstName { get; set; } = "Admin";

    /// <summary>
    /// Gets or sets the default last name for the admin user if not available from identity provider.
    /// </summary>
    public string DefaultLastName { get; set; } = "User";
}
