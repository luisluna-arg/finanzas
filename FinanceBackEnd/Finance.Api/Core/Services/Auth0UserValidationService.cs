using System.Text.Json.Serialization;
using Auth0.ManagementApi;
using Finance.Api.Core.Options;
using Microsoft.Extensions.Options;

namespace Finance.Api.Core.Services;

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

/// <summary>
/// Implementation of Auth0 user validation service.
/// </summary>
public class Auth0UserValidationService : IAuth0UserValidationService
{
    private readonly Auth0Options options;
    private readonly ILogger<Auth0UserValidationService> logger;

    public Auth0UserValidationService(
        IOptions<Auth0Options> options,
        ILogger<Auth0UserValidationService> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<bool> ValidateUserExistsAsync(string userId)
    {
        try
        {
            if (!IsConfigurationValid())
            {
                logger.LogWarning("Auth0 configuration is incomplete. Cannot validate user {UserId}", userId);
                return false;
            }

            var userInfo = await GetUserInfoAsync(userId);
            return userInfo != null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating user {UserId} in Auth0", userId);
            return false;
        }
    }

    public async Task<Auth0UserInfo?> GetUserInfoAsync(string userId)
    {
        try
        {
            if (!IsConfigurationValid())
            {
                logger.LogWarning("Auth0 configuration is incomplete. Cannot get user info for {UserId}", userId);
                return null;
            }

            var managementApiClient = await GetManagementApiClientAsync();
            if (managementApiClient == null)
            {
                return null;
            }

            var user = await managementApiClient.Users.GetAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User {UserId} not found in Auth0", userId);
                return null;
            }

            return new Auth0UserInfo
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.FullName,
                GivenName = user.FirstName,
                FamilyName = user.LastName,
                EmailVerified = user.EmailVerified ?? false
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user info for {UserId} from Auth0", userId);
            return null;
        }
    }

    private bool IsConfigurationValid()
    {
        return !string.IsNullOrWhiteSpace(options.Domain) &&
               !string.IsNullOrWhiteSpace(options.ManagementApi.ClientId) &&
               !string.IsNullOrWhiteSpace(options.ManagementApi.ClientSecret);
    }

    private async Task<ManagementApiClient?> GetManagementApiClientAsync()
    {
        try
        {
            var token = await GetManagementApiTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                logger.LogWarning("Failed to obtain Auth0 Management API token");
                return null;
            }

            var managementApiClient = new ManagementApiClient(token, options.Domain!);
            return managementApiClient;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating Auth0 Management API client");
            return null;
        }
    }

    private async Task<string?> GetManagementApiTokenAsync()
    {
        try
        {
            var httpClient = new HttpClient();
            var tokenRequest = new
            {
                client_id = options.ManagementApi.ClientId,
                client_secret = options.ManagementApi.ClientSecret,
                audience = $"https://{options.Domain}/api/v2/",
                grant_type = "client_credentials"
            };

            var response = await httpClient.PostAsJsonAsync(
                $"https://{options.Domain}/oauth/token",
                tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to obtain Auth0 Management API token. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();
            return tokenResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error obtaining Auth0 Management API token");
            return null;
        }
    }
}
