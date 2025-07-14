using Microsoft.AspNetCore.Http;

namespace CQRSDispatch;

/// <summary>
/// Provides execution context information for command and query operations.
/// Contains HTTP request information that can be used during command execution.
/// </summary>
/// <param name="request">The HTTP request associated with this execution context.</param>
public class ExecutionContext(HttpRequest request)
{
    /// <summary>
    /// Gets or sets the HTTP request associated with this execution context.
    /// </summary>
    public HttpRequest? HttpRequest { get; set; } = request;
}