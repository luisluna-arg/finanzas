using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CQRSDispatch.Strategies;

/// <summary>
/// Asynchronous builder for DispatchContext, extracts user claim and HTTP request data.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
public class DispatchContextBuilderAsync<TContext> : IDispatchContextBuilderAsync<TContext>
    where TContext : DispatchContext, new()
{
    /// <summary>
    /// Builds a new context from the provided HTTP request.
    /// </summary>
    /// <param name="httpRequest">The HTTP request to extract context from.</param>
    /// <returns>A new context instance populated with request and user claim data.</returns>
    public virtual Task<TContext> BuildAsync(HttpRequest? httpRequest)
    {
        var userIdClaim = httpRequest?.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var context = new TContext
        {
            HttpRequest = httpRequest,
            UserIdClaim = userIdClaim?.Value ?? string.Empty
        };

        return Task.FromResult(context);
    }
}
