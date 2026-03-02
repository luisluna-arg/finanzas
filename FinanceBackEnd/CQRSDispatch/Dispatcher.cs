using CQRSDispatch.Interfaces;
using CQRSDispatch.Telemetry;
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
public sealed class Dispatcher<TContext> : IDispatcher<TContext>
    where TContext : DispatchContext, new()
{
    /// <summary>
    /// Activity source that emits one span per dispatched command/query.
    /// </summary>
    public static readonly TaggedActivitySource DispatchActivity = new("Finance.Api.Dispatcher");

    private readonly ILogger<Dispatcher<TContext>> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IDispatchContextBuilderAsync<TContext> ExecutionContextBuilder;
    private readonly CommandHandlerTypeRegistry HandlerRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dispatcher{TContext}"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <param name="executionContextBuilder">Builder for execution context.</param>
    /// <param name="handlerRegistry">Registry mapping command types to their handler interface types, built at startup.</param>
    public Dispatcher(
        ILogger<Dispatcher<TContext>> logger,
        IServiceProvider serviceProvider,
        IDispatchContextBuilderAsync<TContext> executionContextBuilder,
        CommandHandlerTypeRegistry handlerRegistry)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        ExecutionContextBuilder = executionContextBuilder;
        HandlerRegistry = handlerRegistry;
    }

    /// <summary>
    /// Dispatches a context-aware command with HTTP context, returning a result.
    /// </summary>
    /// <typeparam name="TResult">The result type that inherits from RequestResult.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>The command execution result.</returns>
    public async Task<TResult> DispatchAsync<TResult>(IContextAwareCommand<TContext, TResult> command, HttpRequest? httpRequest)
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
    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
        where TResult : RequestResult
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var commandType = command.GetType();

            using var activity = DispatchActivity.StartActivity(commandType.Name, "dispatch.type", "command");

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
    public async Task<CommandResult> DispatchCommandAsync(IContextAwareCommand<TContext> command, HttpRequest? httpRequest)
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
    public async Task<CommandResult> DispatchCommandAsync(ICommand command)
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var commandType = command.GetType();

            using var activity = DispatchActivity.StartActivity(commandType.Name, "dispatch.type", "command");

            var (handler, handlerType) = ResolveCommandHandler(commandType);

            var executeMethod = handlerType.GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handlerType.Name}");

            var taskObj = executeMethod.Invoke(handler, [command, cancellationToken])
                ?? throw new InvalidOperationException($"Handler ExecuteAsync returned null for {commandType.Name}");

            // Await the task — the concrete return type may be CommandResult, DataResult<T>, etc.
            await (Task)taskObj;

            var resultProperty = taskObj.GetType().GetProperty("Result");
            var result = resultProperty?.GetValue(taskObj) as RequestResult
                ?? throw new InvalidOperationException(
                    $"Handler for {commandType.Name} did not return a RequestResult");

            Logger.LogInformation("Command dispatched successfully: {CommandType}", commandType.Name);

            // If the result is already a CommandResult, return it directly;
            // otherwise wrap any RequestResult (e.g. DataResult<T>) into a CommandResult.
            return result as CommandResult
                ?? new CommandResult(result.IsSuccess, result.ErrorMessage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dispatching command: {CommandType}", command.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Resolves a command handler from DI using the startup-built registry.
    /// Tries all registered handler interface types for the command type.
    /// </summary>
    private (object handler, Type handlerInterfaceType) ResolveCommandHandler(Type commandType)
    {
        var interfaceTypes = HandlerRegistry.GetHandlerInterfaceTypes(commandType);

        if (interfaceTypes != null)
        {
            foreach (var candidateType in interfaceTypes)
            {
                var handler = ServiceProvider.GetService(candidateType);
                if (handler != null)
                    return (handler, candidateType);
            }
        }

        throw new InvalidOperationException(
            $"No command handler has been registered for '{commandType.FullName}'. "
            + $"Ensure a class implementing ICommandHandler<{commandType.Name}> or "
            + $"ICommandHandler<{commandType.Name}, TResult> exists and its assembly "
            + $"is included in AddRequestHandlers().");
    }

    /// <summary>
    /// Dispatches a context-aware query with HTTP context, returning a DataResult.
    /// </summary>
    /// <typeparam name="TResult">The data type returned by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="httpRequest">The HTTP request context.</param>
    /// <returns>A DataResult containing the queried data or error information.</returns>
    public async Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IContextAwareQuery<TContext, TResult> query, HttpRequest? httpRequest)
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
    public async Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IQuery<TResult> query)
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            using var activity = DispatchActivity.StartActivity(query.GetType().Name, "dispatch.type", "query");

            var handler = FindQueryHandlerInHierarchy<TResult>(query, ServiceProvider);
            var executeMethod = handler.GetType().GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handler.GetType().Name}");

            var task = executeMethod.Invoke(handler, [query, cancellationToken]) as Task<DataResult<TResult>>
                ?? throw new InvalidOperationException($"Handler ExecuteAsync method did not return expected Task<DataResult<{typeof(TResult).Name}>>");

            var result = await task;

            Logger.LogInformation("Query dispatched successfully: {QueryType}", query.GetType().Name);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dispatching query: {QueryType}", query.GetType().Name);
            throw;
        }
    }

    private static object FindQueryHandlerInHierarchy<TResult>(IQuery<TResult> query, IServiceProvider provider)
    {
        var type = query.GetType();
        while (type != null && type != typeof(object))
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TResult));
            var handler = provider.GetService(handlerType);
            if (handler != null)
                return handler;
            type = type.BaseType;
        }
        throw new InvalidOperationException($"No handler found for query type {query.GetType().Name} or its base types");
    }

    /// <summary>
    /// Creates a new cancellation token for dispatch operations.
    /// </summary>
    /// <returns>A new CancellationToken instance.</returns>
    private CancellationToken CreateCancellationToken() => new CancellationTokenSource().Token;
}
