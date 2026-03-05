namespace Finance.Authentication.Options;

/// <summary>
/// Configuration options for the admin user.
/// </summary>
public class AdminUserOptions
{
    public const string SectionName = "AdminUser";

    /// <summary>
    /// Gets or sets a value indicating whether admin user seeding is enabled.
    /// </summary>
    public bool EnableSeeding { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 user ID of the admin user.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the default username to use when creating a new admin user.
    /// </summary>
    public string DefaultUsername { get; set; } = "admin";

    /// <summary>
    /// Gets or sets the default first name to use when creating a new admin user.
    /// </summary>
    public string DefaultFirstName { get; set; } = "Admin";

    /// <summary>
    /// Gets or sets the default last name to use when creating a new admin user.
    /// </summary>
    public string DefaultLastName { get; set; } = "User";
}
