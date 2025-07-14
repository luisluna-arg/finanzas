namespace CQRSDispatch.Interfaces;

public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
    where TResult : RequestResult
{
    /// <summary>
    /// Executes the request and returns a RequestResult.
    /// </summary>
    /// <param name="request">The request to execute.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the RequestResult.</returns>
    Task<TResult> ExecuteAsync(TCommand request, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Executes the request and returns a CommandResult.
    /// </summary>
    /// <param name="request">The request to execute.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the CommandResult.</returns>
    Task<CommandResult> ExecuteAsync(TCommand request, CancellationToken cancellationToken = default);
}