using Microsoft.AspNetCore.Http;

namespace CQRSDispatch.Interfaces;

/// <summary>
/// Interface for universal dispatch template that handles both commands and queries using method overloads.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
public interface IDispatcher<TContext>
    where TContext : DispatchContext, new()
{
    /// <summary>
    /// Dispatches a command with HTTP context that returns a result.
    /// </summary>
    /// <typeparam name="TResult">The result type that inherits from RequestResult.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>The command execution result.</returns>
    Task<TResult> DispatchAsync<TResult>(IContextAwareCommand<TContext, TResult> command, HttpRequest? httpRequest)
        where TResult : RequestResult;

    /// <summary>
    /// Dispatches a command without HTTP context that returns a result.
    /// </summary>
    /// <typeparam name="TResult">The result type that inherits from RequestResult.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <returns>The command execution result.</returns>
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
        where TResult : RequestResult;

    /// <summary>
    /// Dispatches a command with HTTP context that returns no data (void command).
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>A CommandResult indicating success or failure.</returns>
    Task<CommandResult> DispatchCommandAsync(IContextAwareCommand<TContext> command, HttpRequest? httpRequest);

    /// <summary>
    /// Dispatches a command without HTTP context that returns no data (void command).
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <returns>A CommandResult indicating success or failure.</returns>
    Task<CommandResult> DispatchCommandAsync(ICommand command);

    /// <summary>
    /// Dispatches a query with HTTP context to retrieve data.
    /// </summary>
    /// <typeparam name="TResult">The data type returned by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>A DataResult containing the queried data or error information.</returns>
    Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IContextAwareQuery<TContext, TResult> query, HttpRequest? httpRequest);

    /// <summary>
    /// Dispatches a query without HTTP context to retrieve data.
    /// </summary>
    /// <typeparam name="TResult">The data type returned by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <returns>A DataResult containing the queried data or error information.</returns>
    Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IQuery<TResult> query);
}
