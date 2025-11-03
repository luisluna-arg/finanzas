namespace CQRSDispatch.Interfaces;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Executes the request and returns a QueryResult.
    /// </summary>
    /// <param name="request">The request to execute.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the QueryResult.</returns>
    Task<DataResult<TResult>> ExecuteAsync(TQuery request, CancellationToken cancellationToken = default);
}
