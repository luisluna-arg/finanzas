using System.Text.Json.Serialization;

namespace Finance.Authentication.Services;

/// <summary>
/// Response model for Auth0 Management API token.
/// </summary>
public class Auth0TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
