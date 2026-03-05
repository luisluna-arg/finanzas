using Microsoft.AspNetCore.Http;

namespace CQRSDispatch;

/// <summary>
/// Interface for asynchronously building a dispatch context from an HTTP request.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
public interface IDispatchContextBuilderAsync<TContext>
    where TContext : DispatchContext, new()
{
    /// <summary>
    /// Builds a dispatch context from the provided HTTP request.
    /// </summary>
    /// <param name="httpRequest">The HTTP request to extract context from.</param>
    /// <returns>A new context instance.</returns>
    Task<TContext> BuildAsync(HttpRequest? httpRequest);
}
