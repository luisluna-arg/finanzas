using CQRSDispatch.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CQRSDispatch;

/// <summary>
/// Universal dispatch template that handles both commands and queries using method overloads
/// </summary>
public class Dispatcher : IDispatcher
{
    protected readonly ILogger<Dispatcher> Logger;
    protected readonly IServiceProvider ServiceProvider;

    public Dispatcher(ILogger<Dispatcher> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    private CancellationToken CreateCancellationToken() => new System.Threading.CancellationTokenSource().Token;

    // Command overloads with return data
    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, HttpRequest httpRequest)
        where TResult : RequestResult
    {
        Logger.LogInformation("Dispatching command: {CommandType}", command.GetType().Name);

        var context = new ExecutionContext(httpRequest);

        if (command is IContextAware contextAwareCommand)
        {
            contextAwareCommand.SetContext(context);
        }

        return await DispatchAsync(command);
    }

    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
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

    // Command overloads without return data
    public async Task<CommandResult> DispatchCommandAsync(ICommand command, HttpRequest httpRequest)
    {
        Logger.LogInformation("Dispatching command: {CommandType}", command.GetType().Name);

        var context = new ExecutionContext(httpRequest);

        if (command is IContextAware contextAwareCommand)
        {
            contextAwareCommand.SetContext(context);
        }

        return await DispatchCommandAsync(command);
    }

    public async Task<CommandResult> DispatchCommandAsync(ICommand command)
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

    // Query overloads
    public async Task<DataResult<TData>> DispatchQueryAsync<TData>(IQuery<TData> query, HttpRequest httpRequest)
    {
        Logger.LogInformation("Dispatching query: {QueryType}", query.GetType().Name);

        var context = new ExecutionContext(httpRequest);

        if (query is IContextAware contextAwareQuery)
        {
            contextAwareQuery.SetContext(context);
        }

        return await DispatchQueryAsync(query);
    }

    public async Task<DataResult<TData>> DispatchQueryAsync<TData>(IQuery<TData> query)
    {
        var cancellationToken = CreateCancellationToken();
        try
        {
            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TData));
            var handler = ServiceProvider.GetRequiredService(handlerType);

            var executeMethod = handlerType.GetMethod("ExecuteAsync")
                ?? throw new InvalidOperationException($"ExecuteAsync method not found on handler type {handlerType.Name}");

            var task = executeMethod.Invoke(handler, [query, cancellationToken]) as Task<DataResult<TData>>
                ?? throw new InvalidOperationException($"Handler ExecuteAsync method did not return expected Task<DataResult<{typeof(TData).Name}>>");

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
}
