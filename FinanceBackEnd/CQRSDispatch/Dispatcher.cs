using CQRSDispatch.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CQRSDispatch;

/// <summary>
/// Generic CQRS dispatcher for commands and queries. Supports context-aware and non-context-aware operations.
/// Uses dependency injection to resolve handlers and builds execution context from HTTP requests.
/// Handles logging and error management for dispatch operations.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
public class Dispatcher<TContext> : IDispatcher<TContext>
    where TContext : DispatchContext, new()
{
    protected readonly ILogger<Dispatcher<TContext>> Logger;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDispatchContextBuilderAsync<TContext> ExecutionContextBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dispatcher{TContext}"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <param name="executionContextBuilder">Builder for execution context.</param>
    public Dispatcher(ILogger<Dispatcher<TContext>> logger, IServiceProvider serviceProvider, IDispatchContextBuilderAsync<TContext> executionContextBuilder)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        ExecutionContextBuilder = executionContextBuilder;
    }

    /// <summary>
    /// Dispatches a context-aware command with HTTP context, returning a result.
    /// </summary>
    /// <typeparam name="TResult">The result type that inherits from RequestResult.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>The command execution result.</returns>
    public virtual async Task<TResult> DispatchAsync<TResult>(IContextAwareCommand<TContext, TResult> command, HttpRequest? httpRequest)
        where TResult : RequestResult
    {
        Logger.LogInformation("Dispatching command: {CommandType}", command.GetType().Name);

        if (command is IContextAware<TContext> contextAwareCommand)
        {
            contextAwareCommand.SetContext(await ExecutionContextBuilder.BuildAsync(httpRequest));
        }

        return await DispatchAsync(command);
    }

    /// <summary>
    /// Dispatches a command without HTTP context, returning a result.
    /// </summary>
    /// <typeparam name="TResult">The result type that inherits from RequestResult.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <returns>The command execution result.</returns>
    public virtual async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
        where TResult : RequestResult
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var handler = ServiceProvider.GetRequiredService(handlerType);

            var executeMethod = handlerType.GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handlerType.Name}");

            var task = executeMethod.Invoke(handler, [command, cancellationToken]) as Task<TResult>
                ?? throw new InvalidOperationException($"Handler ExecuteAsync method did not return expected Task<{typeof(TResult).Name}>");

            var result = await task;

            Logger.LogInformation("Command dispatched successfully: {CommandType}", commandType.Name);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dispatching command: {CommandType}", command.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Dispatches a context-aware command with HTTP context, returning a CommandResult.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>A CommandResult indicating success or failure.</returns>
    public virtual async Task<CommandResult> DispatchCommandAsync(IContextAwareCommand<TContext> command, HttpRequest? httpRequest)
    {
        Logger.LogInformation("Dispatching command: {CommandType}", command.GetType().Name);

        if (command is IContextAware<TContext> contextAwareCommand)
        {
            contextAwareCommand.SetContext(await ExecutionContextBuilder.BuildAsync(httpRequest));
        }

        return await DispatchCommandAsync(command);
    }

    /// <summary>
    /// Dispatches a command without HTTP context, returning a CommandResult.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <returns>A CommandResult indicating success or failure.</returns>
    public virtual async Task<CommandResult> DispatchCommandAsync(ICommand command)
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(CommandResult));
            var handler = ServiceProvider.GetRequiredService(handlerType);

            var executeMethod = handlerType.GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handlerType.Name}");

            var task = executeMethod.Invoke(handler, [command, cancellationToken]) as Task<CommandResult>
                ?? throw new InvalidOperationException($"Handler ExecuteAsync method did not return expected Task<CommandResult>");

            var result = await task;

            Logger.LogInformation("Command dispatched successfully: {CommandType}", commandType.Name);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dispatching command: {CommandType}", command.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Dispatches a context-aware query with HTTP context, returning a DataResult.
    /// </summary>
    /// <typeparam name="TResult">The data type returned by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>A DataResult containing the queried data or error information.</returns>
    public virtual async Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IContextAwareQuery<TContext, TResult> query, HttpRequest? httpRequest)
    {
        Logger.LogInformation("Dispatching query: {QueryType}", query.GetType().Name);

        if (query is IContextAware<TContext> contextAwareQuery)
        {
            contextAwareQuery.SetContext(await ExecutionContextBuilder.BuildAsync(httpRequest));
        }

        return await DispatchQueryAsync(query);
    }

    /// <summary>
    /// Dispatches a query without HTTP context, returning a DataResult.
    /// </summary>
    /// <typeparam name="TResult">The data type returned by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <returns>A DataResult containing the queried data or error information.</returns>
    public virtual async Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IQuery<TResult> query)
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            var handler = ServiceProvider.GetRequiredService(handlerType);

            var executeMethod = handlerType.GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handlerType.Name}");

            var task = executeMethod.Invoke(handler, [query, cancellationToken]) as Task<DataResult<TResult>>
                ?? throw new InvalidOperationException($"Handler ExecuteAsync method did not return expected Task<DataResult<{typeof(TResult).Name}>>");

            var result = await task;

            Logger.LogInformation("Query dispatched successfully: {QueryType}", queryType.Name);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dispatching query: {QueryType}", query.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Creates a new cancellation token for dispatch operations.
    /// </summary>
    /// <returns>A new CancellationToken instance.</returns>
    protected CancellationToken CreateCancellationToken() => new CancellationTokenSource().Token;
}
