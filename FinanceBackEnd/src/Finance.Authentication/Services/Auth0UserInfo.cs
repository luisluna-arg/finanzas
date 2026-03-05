namespace Finance.Authentication.Services;

/// <summary>
/// User information from Auth0.
/// </summary>
public class Auth0UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public bool EmailVerified { get; set; }
}
