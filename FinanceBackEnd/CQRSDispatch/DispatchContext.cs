using Microsoft.AspNetCore.Http;

namespace CQRSDispatch;

/// <summary>
/// Provides dispatch context information for command and query operations.
/// Contains HTTP request and user claim information used during command execution.
/// </summary>
public class DispatchContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchContext"/> class.
    /// </summary>
    public DispatchContext()
    {
        HttpRequest = null;
        UserIdClaim = string.Empty;
    }

    /// <summary>
    /// Gets or sets the HTTP request associated with this dispatch context.
    /// </summary>
    public HttpRequest? HttpRequest { get; set; }

    /// <summary>
    /// Gets or sets the user ID claim associated with this dispatch context.
    /// </summary>
    public string UserIdClaim { get; set; }
}